using System;
using Newtonsoft.Json;

namespace Fish
{
    public class UnityVector3JsonConverter:JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var vec3 = (UnityEngine.Vector3) value;
            writer.WriteStartObject();
            WriteFloat(writer, "x", vec3.x);
            WriteFloat(writer, "y", vec3.y);
            WriteFloat(writer, "z", vec3.z);
            writer.WriteEndObject();
        }

        private void WriteFloat(JsonWriter writer,string name, float value)
        {
            if (Math.Abs(value) < float.Epsilon) return;
            
            writer.WritePropertyName(name);
            writer.WriteValue(value);            
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(UnityEngine.Vector3) == objectType;
        }
    }
}