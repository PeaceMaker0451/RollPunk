using NetcodeCommon;
using RollPunk.Entities;
using RollPunk.Fields;
using RollPunk.HierarchyFields;
using RollPunk.Players;
using RollPunk.UIFields;
using System;
using System.Collections.Generic;
using Xunit;

namespace RollPunk.Tests
{
    public class SessionTests
    {
        private Session CreateTestSession()
        {
            var entityFactory = new EntityFactory();
            entityFactory.RegisterFields();
            entityFactory.RegisterHierarchyFields();
            entityFactory.RegisterLineFields();
            entityFactory.RegisterPlayers();
            
            return new Session(entityFactory);
        }

        [Fact]
        public void AddPlayer_ValidData_PlayerAdded()
        {
            // Arrange
            var session = CreateTestSession();
            var clientId = Guid.NewGuid();
            var playerName = "TestPlayer";

            // Act
            var player = session.AddPlayer(clientId, playerName);

            // Assert
            Assert.NotNull(player);
            Assert.Equal(playerName, player.Name);
            Assert.True(session.Players.ContainsKey(clientId));
            Assert.Equal(player, session.Players[clientId]);
        }

        [Fact]
        public void AddPlayer_DuplicateId_ThrowsException()
        {
            // Arrange
            var session = CreateTestSession();
            var clientId = Guid.NewGuid();
            session.AddPlayer(clientId, "Player1");

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => 
                session.AddPlayer(clientId, "Player2"));
        }

        [Fact]
        public void RemovePlayer_ExistingPlayer_PlayerRemoved()
        {
            // Arrange
            var session = CreateTestSession();
            var clientId = Guid.NewGuid();
            var addedPlayer = session.AddPlayer(clientId, "TestPlayer");

            // Act
            var removedPlayer = session.RemovePlayer(clientId);

            // Assert
            Assert.NotNull(removedPlayer);
            Assert.Equal(addedPlayer.Name, removedPlayer.Name);
            Assert.False(session.Players.ContainsKey(clientId));
        }

        [Fact]
        public void RemovePlayer_NonExistentPlayer_ReturnsNull()
        {
            // Arrange
            var session = CreateTestSession();
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = session.RemovePlayer(nonExistentId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void ApplySessionPatch_AddPlayers_PlayersAdded()
        {
            // Arrange
            var session = CreateTestSession();
            var clientId = Guid.NewGuid();
            
            // Создаем Player правильным способом и получаем его состояние
            var player = new Player("TestPlayer", clientId, false);
            var playerState = player.GetState();

            var patch = new SessionPatch
            {
                PendingPlayers = new Dictionary<Guid, EntityState> { { clientId, playerState } }
            };

            // Act
            session.ApplySessionPatch(patch);

            // Assert
            Assert.True(session.Players.ContainsKey(clientId));
            Assert.Equal("TestPlayer", session.Players[clientId].Name);
        }

        [Fact]
        public void ApplySessionPatch_RemovePlayers_PlayersRemoved()
        {
            // Arrange
            var session = CreateTestSession();
            var clientId = Guid.NewGuid();
            session.AddPlayer(clientId, "TestPlayer");

            var patch = new SessionPatch
            {
                RemovePlayers = new List<Guid> { clientId }
            };

            // Act
            session.ApplySessionPatch(patch);

            // Assert
            Assert.False(session.Players.ContainsKey(clientId));
        }

        [Fact]
        public void GetState_WithPlayers_ReturnsCorrectState()
        {
            // Arrange
            var session = CreateTestSession();
            var clientId = Guid.NewGuid();
            session.AddPlayer(clientId, "TestPlayer");

            // Act
            var state = session.GetState();

            // Assert
            Assert.NotNull(state);
            Assert.NotNull(state.Fields);
            Assert.NotNull(state.Players);
            Assert.True(state.Players.ContainsKey(clientId));
        }
    }
}
