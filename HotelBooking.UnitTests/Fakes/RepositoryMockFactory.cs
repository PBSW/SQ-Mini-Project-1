using System.Collections.Generic;
using HotelBooking.Core;
using Moq;

namespace HotelBooking.UnitTests.Fakes;

public static class RepositoryMockFactory
{
    public static Mock<IRepository<T>> CreateMockRepository<T>(IEnumerable<T> initialData = null)
    {
        var mockRepo = new Mock<IRepository<T>>();

        // Local data store to simulate repository state
        var dataStore = initialData != null ? new List<T>(initialData) : new List<T>();

        // Setup GetAll() method
        mockRepo.Setup(repo => repo.GetAll()).Returns(() => dataStore);

        // Setup Get(id) method
        mockRepo.Setup(repo => repo.Get(It.IsAny<int>()))
            .Returns<int>(id => dataStore.Count > id ? dataStore[id] : default);

        // Setup Add(entity) method
        mockRepo.Setup(repo => repo.Add(It.IsAny<T>()))
            .Callback<T>(entity => dataStore.Add(entity));

        // Setup Edit(entity) method
        mockRepo.Setup(repo => repo.Edit(It.IsAny<T>()))
            .Callback<T>(entity =>
            {
                var index = dataStore.IndexOf(entity);
                if (index != -1)
                {
                    dataStore[index] = entity;
                }
            });

        // Setup Remove(id) method
        mockRepo.Setup(repo => repo.Remove(It.IsAny<int>()))
            .Callback<int>(id =>
            {
                if (id >= 0 && id < dataStore.Count)
                {
                    dataStore.RemoveAt(id);
                }
            });

        return mockRepo;
    }
}