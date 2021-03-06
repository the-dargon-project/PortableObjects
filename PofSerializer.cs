using System;
using System.IO;
using System.Text;
using ItzWarty;
using ItzWarty.IO;

namespace Dargon.PortableObjects
{
   public class PofSerializer : IPofSerializer {
      private readonly IPofContext context;
      private static readonly SlotSourceFactoryInternal slotSourceFactory = new SlotSourceFactoryInternalImpl();

      public PofSerializer(IPofContext context)
      {
         this.context = context;
      }

      public void Serialize<T>(Stream stream, T portableObject) {
         using (var writer = new BinaryWriter(stream, Encoding.UTF8, true)) {
            Serialize(writer, portableObject);
         }
      }

      public void Serialize(Stream stream, object portableObject) {
         using (var writer = new BinaryWriter(stream, Encoding.UTF8, true)) {
            Serialize(writer, (object)portableObject);
         }
      }

      public void Serialize<T>(BinaryWriter writer, T portableObject) { 
         Serialize(writer, (object)portableObject);
      }

      public void Serialize(BinaryWriter writer, object portableObject) {
         Serialize(writer, portableObject, SerializationFlags.Default);
      }

      public void Serialize(BinaryWriter writer, object portableObject, SerializationFlags serializationFlags) {
         var slotDestination = new SlotDestination();
         var pofWriter = new PofWriter(context, slotDestination);

         if (serializationFlags.HasFlag(SerializationFlags.Typeless)) {
            pofWriter.WriteObjectTypeless(0, portableObject);
         } else {
            pofWriter.WriteObject(0, portableObject);
         }

         var data = slotDestination[0];
         writer.Write((int)data.Length);
         writer.Write(data);
      }

      public void Serialize<T>(IBinaryWriter writer, T portableObject) {
         Serialize(writer.__Writer, portableObject);
      }

      public void Serialize(IBinaryWriter writer, object portableObject) {
         Serialize(writer.__Writer, portableObject);
      }

      public void Serialize(IBinaryWriter writer, object portableObject, SerializationFlags serializationFlags) {
         Serialize(writer.__Writer, portableObject, serializationFlags);
      }

      public T Deserialize<T>(Stream stream) { return (T)Deserialize(stream); }

      public object Deserialize(Stream stream) {
         using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
            return Deserialize(reader);
      }

      public T Deserialize<T>(IBinaryReader reader) { return Deserialize<T>(reader.__Reader); }

      public T Deserialize<T>(BinaryReader reader) { return (T)Deserialize(reader); }

      public object Deserialize(IBinaryReader reader) { return Deserialize(reader.__Reader); }

      public object Deserialize(BinaryReader reader) { return Deserialize(reader, SerializationFlags.Default, null); }

      public object Deserialize(IBinaryReader reader, SerializationFlags serializationFlags, Type type) {
         return Deserialize(reader.__Reader, serializationFlags, type);
      }

      public object Deserialize(BinaryReader reader, SerializationFlags serializationFlags, Type type) {
         var data = ReadPofFrame(reader, serializationFlags);
         var pofReader = new PofReader(context, slotSourceFactory.CreateWithSingleSlot(data));
         if (serializationFlags.HasFlag(SerializationFlags.Typeless)) {
            return pofReader.ReadObjectTypeless(0, type);
         } else {
            return pofReader.ReadObject(0);
         }
      }

      private static byte[] ReadPofFrame(BinaryReader reader, SerializationFlags serializationFlags) {
         byte[] data;
         if (serializationFlags.HasFlag(SerializationFlags.Lengthless)) {
            data = reader.ReadAllBytes();
         } else {
            var dataLength = reader.ReadInt32();
            data = reader.ReadBytes(dataLength);
         }
         return data;
      }
   }
}