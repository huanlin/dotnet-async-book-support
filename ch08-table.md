# 第 8 章的表格

## 集合類別總覽

以下表格整理了本章提到的集合，以及幾個雖然沒細講、但值得知道的相關類別：

| 集合類別 | 命名空間 | 特性與適用場景 |
| --- | --- | ---- |
| `ConcurrentDictionary<K,V>` | `System.Collections.Concurrent` | 執行緒安全的字典；提供原子性的 `GetOrAdd`、`AddOrUpdate` 等方法，適合共享快取。 |
| `ConcurrentQueue<T>` | `System.Collections.Concurrent` | 執行緒安全的 FIFO 佇列；適合生產者-消費者模式。 |
| `ConcurrentStack<T>` | `System.Collections.Concurrent` | 執行緒安全的 LIFO 堆疊。 |
| `ConcurrentBag<T>` | `System.Collections.Concurrent` | 無序集合；生產與消費發生在同一執行緒時效能最佳。 |
| `BlockingCollection<T>` | `System.Collections.Concurrent` | 傳統的生產者-消費者包裝類別，支援邊界與阻塞。現代開發建議改用 `Channel<T>`（見第 8 章）。 |
| `ImmutableList<T>` | `System.Collections.Immutable` | 不可變有序串列；任何「修改」皆回傳新集合，天生執行緒安全。 |
| `ImmutableArray<T>` | `System.Collections.Immutable` | 不可變陣列，實值型別（struct），內部為連續記憶體，隨機存取速度快。適合唯讀、固定大小的場景；但新增/刪除成本高。 |
| `ImmutableDictionary<K,V>` | `System.Collections.Immutable` | 不可變字典；適合唯讀組態或快照（snapshot）傳遞。 |
| `FrozenDictionary<K,V>` / `FrozenSet<T>` | `System.Collections.Frozen` | 建立一次後針對查詢最佳化的唯讀集合；適合啟動時載入、之後大量讀取的場景。 |
