using RollPunk.NetcodeCommon;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RollPunk.Tests
{
    public class ThreadManagerTests
    {
        [Fact]
        public void ExecuteOnMainThread_SingleAction_ExecutedCorrectly()
        {
            // Arrange
            var threadManager = new ThreadManager();
            bool actionExecuted = false;

            // Act
            threadManager.ExecuteOnMainThread(() => actionExecuted = true);
            threadManager.UpdateMain();

            // Assert
            Assert.True(actionExecuted);
        }

        [Fact]
        public void ExecuteOnMainThread_MultipleActions_ExecutedInOrder()
        {
            // Arrange
            var threadManager = new ThreadManager();
            var executionOrder = new List<int>();

            // Act
            threadManager.ExecuteOnMainThread(() => executionOrder.Add(1));
            threadManager.ExecuteOnMainThread(() => executionOrder.Add(2));
            threadManager.ExecuteOnMainThread(() => executionOrder.Add(3));
            threadManager.UpdateMain();

            // Assert
            Assert.Equal(3, executionOrder.Count);
            Assert.Equal(1, executionOrder[0]);
            Assert.Equal(2, executionOrder[1]);
            Assert.Equal(3, executionOrder[2]);
        }

        [Fact]
        public void ExecuteOnMainThread_ConcurrentCalls_ThreadSafe()
        {
            // Arrange
            var threadManager = new ThreadManager();
            var executedCount = 0;
            var tasks = new List<Task>();

            // Act - запускаем несколько потоков одновременно
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    threadManager.ExecuteOnMainThread(() => Interlocked.Increment(ref executedCount));
                }));
            }

            Task.WaitAll(tasks.ToArray());
            threadManager.UpdateMain();

            // Assert
            Assert.Equal(10, executedCount);
        }

        [Fact]
        public void ExecuteOnMainThread_NullAction_HandledGracefully()
        {
            // Arrange
            var threadManager = new ThreadManager();

            // Act & Assert - не должно выбрасывать исключение
            threadManager.ExecuteOnMainThread(null);
            threadManager.UpdateMain(); // Должно работать без ошибок
        }

        [Fact]
        public void UpdateMain_MultipleUpdates_ClearsQueue()
        {
            // Arrange
            var threadManager = new ThreadManager();
            int executionCount = 0;

            // Act
            threadManager.ExecuteOnMainThread(() => executionCount++);
            threadManager.UpdateMain();
            
            // Добавляем еще одно действие после первого обновления
            threadManager.ExecuteOnMainThread(() => executionCount++);
            threadManager.UpdateMain();

            // Assert
            Assert.Equal(2, executionCount);
        }

        [Fact]
        public void ExecuteOnMainThread_ExceptionInAction_DoesNotBreakQueue()
        {
            // Arrange
            var threadManager = new ThreadManager();
            bool secondActionExecuted = false;

            // Act
            threadManager.ExecuteOnMainThread(() => throw new Exception("Test exception"));
            threadManager.ExecuteOnMainThread(() => secondActionExecuted = true);

            // Assert - второе действие должно выполниться несмотря на исключение в первом
            try
            {
                threadManager.UpdateMain();
                threadManager.UpdateMain();
            }
            catch
            {
                // Игнорируем исключение от первого действия
            }

            Assert.True(secondActionExecuted);
        }
    }
}
