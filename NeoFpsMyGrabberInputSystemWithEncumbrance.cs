// Alternative version for use with the input system extension
/*

using System;
using System.Collections;
using UnityEngine;
using NeoFPS.CharacterMotion;
using NeoFPS.CharacterMotion.MotionData;
using NeoFPS.CharacterMotion.Parameters;
using UnityEngine.Serialization;
using NeoSaveGames.Serialization;
using NeoSaveGames;

namespace NeoFPS.Grabber
{
    [RequireComponent(typeof(BaseCharacter))]
    public class NeoFpsMyGrabberInputSystemWithEncumbrance : NeoFpsMyGrabberInputSystem, IMotionGraphDataOverride
    {
        [Header("Encumbrance")]

        [SerializeField, MotionGraphParameterKey(MotionGraphParameterType.Switch), Tooltip("The optional motion graph parameter to set when encumbered.")]
        private string m_EncumberedParameter = string.Empty;
        [SerializeField, Tooltip("The multiplier applied to movement speed when carrying something with a mass at the character's limit.")]
        private float m_FullEncumberedMultiplier = 0.2f;
        [SerializeField, FormerlySerializedAs("m_GraphParameters"), MotionGraphDataKey(MotionGraphDataType.Float), Tooltip("The motion data parameters to apply the multiplier to.")]
        private string[] m_GraphData = { };

        float m_Multiplier = 1f;
        SwitchParameter m_EncumberedParam = null;

        protected override void Start()
        {
            base.Start();

            var mg = m_Character.motionController.motionGraph;

            mg.AddDataOverrides(this);

            if (!string.IsNullOrWhiteSpace(m_EncumberedParameter))
                m_EncumberedParam = mg.GetSwitchProperty(m_EncumberedParameter);
        }

        public Func<bool, bool> GetBoolOverride(BoolData data)
        {
            return null;
        }

        public Func<float, float> GetFloatOverride(FloatData data)
        {
            // Iterate through list of data keys, and use override method for any that match
            for (int i = 0; i < m_GraphData.Length; ++i)
            {
                if (data.name == m_GraphData[i])
                    return GetModifiedSpeed;
            }
            return null;
        }

        public Func<int, int> GetIntOverride(IntData data)
        {
            return null;
        }

        float GetModifiedSpeed(float input)
        {
            return m_Multiplier * input;
        }

        protected override void ObjectConnected(GameObject obj)
        {
            base.ObjectConnected(obj);

            // Set multiplier based on object's mass
            var rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
                m_Multiplier = Mathf.Lerp(1f, m_FullEncumberedMultiplier, rb.mass / massLimit);

            // Set encumbered parameter value
            if (m_EncumberedParam != null)
                m_EncumberedParam.on = true;
        }

        protected override void ObjectDisconnected()
        {
            base.ObjectDisconnected();

            // Reset multiplier to 1
            m_Multiplier = 1f;

            // Set encumbered parameter value
            if (m_EncumberedParam != null)
                m_EncumberedParam.on = false;
        }

        #region SAVE GAMES

        static readonly NeoSerializationKey k_SpeedMultiplierKey = new NeoSerializationKey("speedMultiplier");

        public override void WriteProperties(INeoSerializer writer, NeoSerializedGameObject nsgo, SaveMode saveMode)
        {
            base.WriteProperties(writer, nsgo, saveMode);

            if (myGrabber.grabbedObj != null)
                writer.WriteValue(k_SpeedMultiplierKey, m_Multiplier);
        }

        public override void ReadProperties(INeoDeserializer reader, NeoSerializedGameObject nsgo)
        {
            base.ReadProperties(reader, nsgo);

            if (myGrabber.grabbedObj != null)
                reader.TryReadValue(k_SpeedMultiplierKey, out m_Multiplier, m_Multiplier);
        }

        #endregion
    }
}

*/