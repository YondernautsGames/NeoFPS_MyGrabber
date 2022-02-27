// Alternative version for use with the input system extension
/*

using UnityEngine;
using MyGrabber3D;
using UnityEngine.Events;
using NeoSaveGames.Serialization;
using NeoSaveGames;
using System.Collections;
using UnityEngine.InputSystem;

namespace NeoFPS.Grabber
{
    [RequireComponent (typeof(BaseCharacter))]
    public class NeoFpsMyGrabberInputSystem : CharacterInputHandlerBase, IMyGrabberInputHandler, INeoSerializableComponent
    {
        [SerializeField, Tooltip("The maximum distance you can reach forwards to grab an object.")] 
		private float m_MaxReach = 3.5f;
		[SerializeField, Tooltip("The maximum mass of the object you can carry.")]
		private float m_MassLimit = 75.1f;
		[SerializeField, Tooltip("The layer to grab objects from.")]
		private LayerMask m_PickupLayer = PhysicsFilter.LayerFilter.DynamicProps;
		[SerializeField, Tooltip("The time between physics tests in front of the camera.")]
		private float m_TickInterval = 0.25f;
		[SerializeField, Tooltip("The number of degrees per second to turn the object at full analog turn.")]
		private float m_AnalogRotateRate = 60f;

		[Header("Damage Interrupt")]

		[SerializeField, Tooltip("If the character recieves damage higher than this value in one go, then they will drop the object.")]
		private float m_DropOnDamage = 10f;
		[SerializeField, Tooltip("If the character recieves damage totalling this value since picking up the object, they will drop it.")]
		private float m_MaxTotalDamage = 10f;

		private IHealthManager m_HealthManager = null;
        private bool m_LockCamera = false;
		private float m_TotalDamage = 0f;
		private float m_TickTimer = 0f;
		private RaycastHit m_HitInfo = new RaycastHit();

		public static event UnityAction<GrabState> onGrabStateChanged;

		public enum GrabState
		{
			Inactive,
			ValidTarget,
			TargetTooHeavy,
			Grabbed
		}

		private GrabState m_GrabState = GrabState.Inactive;
		public GrabState grabState
        {
			get { return m_GrabState; }
			private set
            {
				if (m_GrabState != value)
                {
					m_GrabState = value;
					onGrabStateChanged?.Invoke(m_GrabState);
                }
            }
		}

		public MyGrabber myGrabber
        {
			get;
			private set;
        }

		public float massLimit
        {
			get { return m_MassLimit; }
			protected set { m_MassLimit = value; }
        }

		public bool cameraInputs
        {
            get { return !m_LockCamera; }
            set
            {
                if (m_LockCamera == value)
                {
                    m_LockCamera = !value;

                    var aim = m_Character.aimController;
                    if (m_LockCamera)
                    {
                        aim.SetPitchConstraints(aim.pitch, aim.pitch);
                        aim.SetYawConstraints(aim.forward, 0f);
                    }
                    else
                    {
                        aim.ResetPitchConstraints();
                        aim.ResetYawConstraints();
                    }
                }
            }
        }

		void OnValidate()
        {
			m_MaxReach = Mathf.Clamp(m_MaxReach, 0.5f, 10f);
			m_MassLimit = Mathf.Clamp(m_MassLimit, 1f, 500f);
			m_AnalogRotateRate = Mathf.Clamp(m_AnalogRotateRate, 0.1f, 100f);
			m_DropOnDamage = Mathf.Clamp(m_DropOnDamage, 1f, 1000f);
			m_MaxTotalDamage = Mathf.Clamp(m_MaxTotalDamage, 1f, 1000f);
		}

        protected override void OnAwake()
        {
            base.OnAwake();

            myGrabber = m_Character.fpCamera.GetComponentInChildren<MyGrabber>(true);
            if (myGrabber == null)
                enabled = false;
			else
            {
				// Attach to MyGrabber events
				myGrabber.onObjectGrabbed += ObjectConnected;
				myGrabber.onObjectDropped += ObjectDisconnected;
				myGrabber.onObjectThrown += ObjectDisconnected;
				myGrabber.onObjectDestroyed += ObjectDisconnected;

				// Get health manager
				m_HealthManager = m_Character.GetComponent<IHealthManager>();

				// Get 
				if (myGrabber.grabbedObj != null)
					grabState = GrabState.Grabbed;
			}
		}

		protected override void UpdateInput()
		{
			if (!myGrabber.enabled)
				return;

			if (myGrabber.grabbedObj == null)
			{
				// Intermittent physics checks
				m_TickTimer -= Time.unscaledTime;
                if (m_TickTimer <= 0f)
                {
					// Raycast and check hit target is valid
					if (Physics.Raycast(new Ray(m_Character.fpCamera.aimTransform.position, m_Character.fpCamera.aimTransform.forward), out m_HitInfo, m_MaxReach, m_PickupLayer) && m_HitInfo.rigidbody != null)
					{
						if (m_HitInfo.rigidbody.mass <= m_MassLimit)
						{
							Debug.DrawLine(Camera.main.transform.position, m_HitInfo.point, Color.red);
							grabState = GrabState.ValidTarget;
						}
						else
							grabState = GrabState.TargetTooHeavy;
					}
					else
						grabState = GrabState.Inactive;

					// Reset tick timer
					m_TickTimer = m_TickInterval;
                }

				// Check for input & grab if valid target
				if (grabState == GrabState.ValidTarget && GetButtonDown(NeoFpsNewInputManager.controls.Interaction.PickUp))
					myGrabber.GrabObject(m_HitInfo.collider.gameObject);
			}
			else
			{
				// Rotate the object
				if (GetButton(NeoFpsNewInputManager.controls.Combat.SecondaryFire))
				{
					// Lock camera
					cameraInputs = false;
					var lookVector = NeoFpsNewInputManager.isGamepadConnected ? NeoFpsNewInputManager.controls.Movement.AnalogueLook.ReadValue<Vector2>() : NeoFpsNewInputManager.controls.Movement.MouseLook.ReadValue<Vector2>();
					var LookAxis = NeoFpsNewInputManager.isGamepadConnected ? Gamepad.current.rightStick.ReadValue() : Mouse.current.position.ReadValue();
					
					
					// Get mouse & analog
					float x = LookAxis.x * FpsSettings.input.horizontalMouseSensitivity + lookVector.x * m_AnalogRotateRate * Time.deltaTime;
					float y = LookAxis.y * FpsSettings.input.verticalMouseSensitivity + lookVector.y * m_AnalogRotateRate * Time.deltaTime;

					// Rotate
					myGrabber.RotateObject(x, y);
				}
				else
				{
					// Unlock camera
					cameraInputs = true;
				}

                // Check for throw input
                if (GetButtonDown(NeoFpsNewInputManager.controls.Combat.PrimaryFire))
                    myGrabber.ThrowObject();

                // Check for release input
                if (GetButtonDown(NeoFpsNewInputManager.controls.Interaction.PickUp))
					myGrabber.ReleaseObject();
			}
		}

		#region EVENT HANDLERS

		protected virtual void ObjectDisconnected()
		{
			// Raise weapon
			if (m_Character.quickSlots != null)
				m_Character.quickSlots.UnlockSelection(this);

			// Unsubscribe from health events
			if (m_HealthManager != null)
			{
				m_HealthManager.onHealthChanged -= OnHealthChanged;
				m_HealthManager.onIsAliveChanged -= OnIsAliveChanged;
			}

			// Reset tick timer
			m_TickTimer = 0f;

			// Set grab state
			grabState = GrabState.Inactive;
		}

		private void ObjectDisconnected(GameObject obj)
		{
			ObjectDisconnected();
		}

		protected virtual void ObjectConnected(GameObject obj)
		{
			// Lower weapon
			if (m_Character.quickSlots != null)
				m_Character.quickSlots.LockSelectionToNothing(this, false);

			// Subscribe to health events
			if (m_HealthManager != null)
			{
				m_HealthManager.onHealthChanged += OnHealthChanged;
				m_HealthManager.onIsAliveChanged += OnIsAliveChanged;
				m_TotalDamage = 0f;
			}

			// Set grab state
			grabState = GrabState.Grabbed;
		}

		void OnHealthChanged(float from, float to, bool critical, IDamageSource source)
		{
			// Get damage and total damage
			float damage = from - to;
			m_TotalDamage += damage;

			// Drop if either pass threshold
			if (damage > m_DropOnDamage || m_TotalDamage > m_MaxTotalDamage)
				myGrabber.ReleaseObject();
		}

		void OnIsAliveChanged(bool alive)
		{
			// Drop on death
			if (myGrabber.grabbedObj != null)
				myGrabber.ReleaseObject();
		}

		#endregion

		#region SAVE GAMES

		static readonly NeoSerializationKey k_GrabbedObjectKey = new NeoSerializationKey("grabbedObject");
		static readonly NeoSerializationKey k_TotalDamageKey = new NeoSerializationKey("damage");

		public virtual void WriteProperties(INeoSerializer writer, NeoSerializedGameObject nsgo, SaveMode saveMode)
		{
			if (myGrabber.grabbedObj != null)
			{
				var grabbedNsgo = myGrabber.grabbedObj.GetComponent<NeoSerializedGameObject>();
				if (grabbedNsgo != null)
				{
					Debug.Log("Saving grabbed object: " + grabbedNsgo.gameObject);
					writer.WriteNeoSerializedGameObjectReference(k_GrabbedObjectKey, grabbedNsgo, nsgo);
					writer.WriteValue(k_TotalDamageKey, m_TotalDamage);
				}
			}
		}

		public virtual void ReadProperties(INeoDeserializer reader, NeoSerializedGameObject nsgo)
		{
			NeoSerializedGameObject grabbed = null;
			if (reader.TryReadNeoSerializedGameObjectReference(k_GrabbedObjectKey, out grabbed, nsgo))
			{
				bool didUseGravity = true;

				// Make the object stationary
				var rb = grabbed.GetComponent<Rigidbody>();
				if (rb != null)
				{
					rb.velocity = Vector3.zero;
					didUseGravity = rb.useGravity;
					rb.useGravity = false;
				}

				// Get total damage
				reader.TryReadValue(k_TotalDamageKey, out m_TotalDamage, m_TotalDamage);

				// Delayed grab
				StartCoroutine(DelayedGrabObject(grabbed.gameObject, m_TotalDamage, didUseGravity));
            }
		}

		IEnumerator DelayedGrabObject(GameObject obj, float totalDamage, bool grav)
        {
			yield return null;
			yield return null;
			yield return null;

			// Grab the object
			myGrabber.GrabObject(obj);

			// Reapply gravity
			var rb = obj.GetComponent<Rigidbody>();
			if (rb != null)
				rb.useGravity = grav;

			// Update damage value
			m_TotalDamage = totalDamage;
		}

		#endregion
	}
}

*/