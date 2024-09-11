using System;
using System.Collections.Generic;
using System.Linq;
using HotelBooking.Core;
using Moq;

namespace HotelBooking.UnitTests.Fakes;

public static class RepositoryMockFactory
{
    public static Mock<IRepository<T>> CreateMockRepository<T>(IEnumerable<T> initialData = null) where T : class
    {
        var mockRepo = new Mock<IRepository<T>>();

        // Local data store to simulate repository state
        var dataStore = initialData != null ? new List<T>(initialData) : new List<T>();

        // Setup GetAll() method
        mockRepo.Setup(repo => repo.GetAll()).Returns(() => dataStore);

        // Setup Get(id) method for objects with Id property
        mockRepo.Setup(repo => repo.Get(It.IsAny<int>()))
            .Returns<int>(id => dataStore.SingleOrDefault(entity => 
            {
                var property = entity.GetType().GetProperty("Id");
                return property != null && (int)property.GetValue(entity) == id;
            }));

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
                var entityToRemove = dataStore.SingleOrDefault(entity =>
                {
                    var property = entity.GetType().GetProperty("Id");
                    return property != null && (int)property.GetValue(entity) == id;
                });

                if (entityToRemove != null)
                {
                    dataStore.Remove(entityToRemove);
                }
                else 
                {
                    throw new InvalidOperationException();
                }
            });

      

        return mockRepo;
    }
}