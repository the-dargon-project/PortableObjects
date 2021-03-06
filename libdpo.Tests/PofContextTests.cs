﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItzWarty;
using NMockito;
using Xunit;

namespace Dargon.PortableObjects.Tests {
   public class PofContextTests : NMockitoInstance {
      private readonly PofContext testObj;
      public PofContextTests() {
         testObj = new PofContext();
      }

      [Fact]
      public void RegisterPortableObjectType_PositiveUnique_Okay() {
         testObj.RegisterPortableObjectType(1, typeof(DummyClass));
         VerifyNoMoreInteractions();
      }

      [Fact]
      public void RegisterPortableObjectType_PositiveEquivalent_Okay() {
         testObj.RegisterPortableObjectType(1, typeof(DummyClass));
         testObj.RegisterPortableObjectType(1, typeof(DummyClass));
         VerifyNoMoreInteractions();
      }

      [Fact]
      public void RegisterPortableObjectType_PositiveDuplicate_Throws() {
         testObj.RegisterPortableObjectType(1, typeof(DummyClass));
         VerifyNoMoreInteractions();

         Assert.Throws<DuplicatePofIdException>(() =>
            testObj.RegisterPortableObjectType(1, typeof(DummyClass2))
         );
         VerifyNoMoreInteractions();
      }

      [Fact]
      public void HasTypeIdTest() {
         AssertFalse(testObj.HasTypeId(1));
         testObj.RegisterPortableObjectType(1, typeof(DummyClass));
         AssertTrue(testObj.HasTypeId(1));
         VerifyNoMoreInteractions();
      }

      [Fact]
      public void RegisterPortableObjectType_NegativeIdByType_Throws() {
         AssertThrows<ArgumentOutOfRangeException>(
            () => testObj.RegisterPortableObjectType(-1337, typeof(DummyClass))
         );
         VerifyNoMoreInteractions();
      }

      [Fact]
      public void RegisterPortableObjectType_NegativeIdByActivator_Throws() {
         Assert.Throws<ArgumentOutOfRangeException>(
            () => testObj.RegisterPortableObjectType(-1337, () => new DummyClass())
         );
         VerifyNoMoreInteractions();
      }

      [Fact]
      public void RegisterPortableObjectType_ByTypeWithoutParameterlessConstructor_Throws() {
         Assert.Throws<MissingMethodException>(
            () => testObj.RegisterPortableObjectType(1337, typeof(DummyClassWithoutDefaultConstructor))
         );
         VerifyNoMoreInteractions();
      }

      [Fact]
      public void GetTypeOrNull_UnknownType_ReturnsNull() {
         AssertNull(testObj.GetTypeOrNull(13337777));
         VerifyNoMoreInteractions();
      }

      [Fact]
      public void GetTypeIdByType_UnknownType_Throws() {
         Assert.Throws<TypeNotFoundException>(
            () => testObj.GetTypeIdByType(typeof(DummyClass2))
         );
         VerifyNoMoreInteractions();
      }

      public class DummyClass : IPortableObject {
         public void Serialize(IPofWriter writer) {
         }

         public void Deserialize(IPofReader reader) {
         }
      }

      public class DummyClass2 : IPortableObject {
         public void Serialize(IPofWriter writer) {
         }

         public void Deserialize(IPofReader reader) {
         }
      }

      public class DummyClassWithoutDefaultConstructor : IPortableObject {
         public DummyClassWithoutDefaultConstructor(int throwaway) { }

         public void Serialize(IPofWriter writer) {
            throw new NotImplementedException();
         }

         public void Deserialize(IPofReader reader) {
            throw new NotImplementedException();
         }
      }
   }
}
