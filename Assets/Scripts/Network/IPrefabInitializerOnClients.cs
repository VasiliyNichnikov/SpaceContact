using System;

namespace Network
{
    /// <summary>
    /// Синхронизируем создание объекта у всех клиентов
    /// </summary>
    public interface IPrefabInitializerOnClients
    {
        /// <summary>
        /// ulong - prefabId
        /// </summary>
        event Action<ulong>? OnLoaded;
        
        bool IsLoaded { get; }
    }
}