using NetcodeCommon;
using Newtonsoft.Json;
using RollPunk.Entities;
using System;
using System.Collections.Generic;
using Xunit;

namespace RollPunk.Tests
{
    public class SessionSerializationTests
    {
        [Fact]
        public void SessionPatch_Serialize_Deserialize_DataPreserved()
        {
            // Arrange
            var originalPatch = new SessionPatch
            {
                PendingPlayers = new Dictionary<Guid, EntityState>
                {
                    {
                        Guid.NewGuid(),
                        new EntityState
                        {
                            EntityType = "Player",
                            Name = "TestPlayer",
                            ID = Guid.NewGuid(),
                            Payload = new Dictionary<string, Newtonsoft.Json.Linq.JToken>
                            {
                                ["IsAdmin"] = false,
                                ["PlayerID"] = Guid.NewGuid().ToString()
                            }
                        }
                    }
                },
                RemovePlayers = new List<Guid> { Guid.NewGuid() },
                PendingFields = new List<RollPunk.Fields.FieldState>(),
                RemoveFields = new List<Guid> { Guid.NewGuid() }
            };

            // Act
            string json = JsonConvert.SerializeObject(originalPatch);
            var deserializedPatch = JsonConvert.DeserializeObject<SessionPatch>(json);

            // Assert
            Assert.NotNull(deserializedPatch);
            Assert.Equal(originalPatch.PendingPlayers.Count, deserializedPatch.PendingPlayers.Count);
            Assert.Equal(originalPatch.RemovePlayers.Count, deserializedPatch.RemovePlayers.Count);
            Assert.Equal(originalPatch.RemoveFields.Count, deserializedPatch.RemoveFields.Count);
        }

        [Fact]
        public void SessionState_Serialize_Deserialize_DataPreserved()
        {
            // Arrange
            var originalState = new SessionState
            {
                Fields = new List<RollPunk.Fields.FieldState>(),
                Players = new Dictionary<Guid, EntityState>
                {
                    {
                        Guid.NewGuid(),
                        new EntityState
                        {
                            EntityType = "Player",
                            Name = "TestPlayer",
                            ID = Guid.NewGuid(),
                            Payload = new Dictionary<string, Newtonsoft.Json.Linq.JToken>
                            {
                                ["IsAdmin"] = true,
                                ["PlayerID"] = Guid.NewGuid().ToString()
                            }
                        }
                    }
                }
            };

            // Act
            string json = JsonConvert.SerializeObject(originalState);
            var deserializedState = JsonConvert.DeserializeObject<SessionState>(json);

            // Assert
            Assert.NotNull(deserializedState);
            Assert.Equal(originalState.Players.Count, deserializedState.Players.Count);
            Assert.NotNull(deserializedState.Fields);
        }

        [Fact]
        public void SessionPatch_EmptyPatch_SerializesCorrectly()
        {
            // Arrange
            var emptyPatch = new SessionPatch();

            // Act
            string json = JsonConvert.SerializeObject(emptyPatch);
            var deserializedPatch = JsonConvert.DeserializeObject<SessionPatch>(json);

            // Assert
            Assert.NotNull(deserializedPatch);
            Assert.NotNull(deserializedPatch.PendingPlayers);
            Assert.NotNull(deserializedPatch.RemovePlayers);
            Assert.NotNull(deserializedPatch.PendingFields);
            Assert.NotNull(deserializedPatch.RemoveFields);
        }

        [Fact]
        public void EntityState_ComplexPayload_SerializesCorrectly()
        {
            // Arrange
            var entityState = new EntityState
            {
                EntityType = "String",
                Name = "TestField",
                ID = Guid.NewGuid(),
                Payload = new Dictionary<string, Newtonsoft.Json.Linq.JToken>
                {
                    ["Value"] = "Test String Value",
                    ["ViewAccessLevel"] = 0,
                    ["EditAccessLevel"] = 1,
                    ["LinePriority"] = 5
                }
            };

            // Act
            string json = JsonConvert.SerializeObject(entityState);
            var deserializedState = JsonConvert.DeserializeObject<EntityState>(json);

            // Assert
            Assert.NotNull(deserializedState);
            Assert.Equal(entityState.EntityType, deserializedState.EntityType);
            Assert.Equal(entityState.Name, deserializedState.Name);
            Assert.Equal(entityState.ID, deserializedState.ID);
            Assert.Equal(entityState.Payload.Count, deserializedState.Payload.Count);
        }
    }
}
