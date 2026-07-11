using NetcodeCommon;
using RollPunk.Entities;
using RollPunk.Server;
using RollPunk.Client.Runtime;
using System;
using System.Collections.Generic;
using Xunit;

namespace RollPunk.Tests
{
    public class SessionIntegrationTests
    {
        private ServerSession CreateServerSession()
        {
            return new ServerSession();
        }

        [Fact]
        public void ServerClient_AddPlayer_StatesSynced()
        {
            // Arrange
            var serverSession = CreateServerSession();
            var clientId = Guid.NewGuid();
            var playerName = "TestPlayer";

            // Act - добавляем игрока на сервере
            var serverPlayer = serverSession.AddPlayer(clientId, playerName);
            var serverState = serverSession.GetState();

            // Создаем патч для клиента
            var patch = new SessionPatch
            {
                PendingPlayers = new Dictionary<Guid, EntityState>
                {
                    { clientId, serverPlayer.GetState() }
                }
            };

            // Имитируем применение на клиенте через базовый Session
            var clientSession = new Session(new EntityFactory());
            clientSession.ApplySessionPatch(patch);

            // Assert
            Assert.True(serverSession.Players.ContainsKey(clientId));
            Assert.True(clientSession.Players.ContainsKey(clientId));
            Assert.Equal(serverSession.Players[clientId].Name, clientSession.Players[clientId].Name);
        }

        [Fact]
        public void ServerClient_PlayerDisconnect_RemovedFromBoth()
        {
            // Arrange
            var serverSession = CreateServerSession();
            var clientId = Guid.NewGuid();
            serverSession.AddPlayer(clientId, "TestPlayer");

            var clientSession = new Session(new EntityFactory());
            var addPatch = new SessionPatch
            {
                PendingPlayers = new Dictionary<Guid, EntityState>
                {
                    { clientId, serverSession.Players[clientId].GetState() }
                }
            };
            clientSession.ApplySessionPatch(addPatch);

            // Act - удаляем игрока на сервере
            var removedPlayer = serverSession.RemovePlayer(clientId);
            var removePatch = new SessionPatch
            {
                RemovePlayers = new List<Guid> { clientId }
            };
            clientSession.ApplySessionPatch(removePatch);

            // Assert
            Assert.NotNull(removedPlayer);
            Assert.False(serverSession.Players.ContainsKey(clientId));
            Assert.False(clientSession.Players.ContainsKey(clientId));
        }

        [Fact]
        public void MultipleClients_SessionState_AllSynced()
        {
            // Arrange
            var serverSession = CreateServerSession();
            var client1Id = Guid.NewGuid();
            var client2Id = Guid.NewGuid();

            // Act - добавляем двух игроков
            var player1 = serverSession.AddPlayer(client1Id, "Player1");
            var player2 = serverSession.AddPlayer(client2Id, "Player2");

            var serverState = serverSession.GetState();

            // Имитируем синхронизацию с клиентами
            var clientSession1 = new Session(new EntityFactory());
            var clientSession2 = new Session(new EntityFactory());

            clientSession1.ApplyState(serverState);
            clientSession2.ApplyState(serverState);

            // Assert
            Assert.Equal(2, serverSession.Players.Count);
            Assert.Equal(2, clientSession1.Players.Count);
            Assert.Equal(2, clientSession2.Players.Count);

            Assert.True(clientSession1.Players.ContainsKey(client1Id));
            Assert.True(clientSession1.Players.ContainsKey(client2Id));
            Assert.True(clientSession2.Players.ContainsKey(client1Id));
            Assert.True(clientSession2.Players.ContainsKey(client2Id));
        }

        [Fact]
        public void SessionPatch_ApplyMultiple_StateConsistent()
        {
            // Arrange
            var session = new Session(new EntityFactory());
            var client1Id = Guid.NewGuid();
            var client2Id = Guid.NewGuid();

            // Act - применяем несколько патчей
            var patch1 = new SessionPatch
            {
                PendingPlayers = new Dictionary<Guid, EntityState>
                {
                    {
                        client1Id,
                        new EntityState
                        {
                            EntityType = "Player",
                            Name = "Player1",
                            ID = Guid.NewGuid(),
                            Payload = new Dictionary<string, Newtonsoft.Json.Linq.JToken>
                            {
                                ["IsAdmin"] = false,
                                ["PlayerID"] = client1Id.ToString(),
                                ["TeamId"] = string.Empty
                            }
                        }
                    }
                }
            };

            var patch2 = new SessionPatch
            {
                PendingPlayers = new Dictionary<Guid, EntityState>
                {
                    {
                        client2Id,
                        new EntityState
                        {
                            EntityType = "Player",
                            Name = "Player2",
                            ID = Guid.NewGuid(),
                            Payload = new Dictionary<string, Newtonsoft.Json.Linq.JToken>
                            {
                                ["IsAdmin"] = true,
                                ["PlayerID"] = client2Id.ToString(),
                                ["TeamId"] = string.Empty
                            }
                        }
                    }
                }
            };

            session.ApplySessionPatch(patch1);
            session.ApplySessionPatch(patch2);

            // Assert
            Assert.Equal(2, session.Players.Count);
            Assert.True(session.Players.ContainsKey(client1Id));
            Assert.True(session.Players.ContainsKey(client2Id));
            Assert.Equal("Player1", session.Players[client1Id].Name);
            Assert.Equal("Player2", session.Players[client2Id].Name);
        }
    }
}
