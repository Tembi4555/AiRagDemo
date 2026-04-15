using AiRagDemo.Domain.Models;

namespace AiRagDemo.Application.Processing;

/// <summary>
/// Подготовка текста к индексации 
/// </summary>
public sealed class TextChunker
{
    /// <summary>
    /// Разбиение текста на фрагменты
    /// </summary>
    /// <param name="documentName">Наименование документа</param>
    /// <param name="text">Текст который будет разбит на фрагменты</param>
    /// <param name="chunkSize">Размер одного отрывка</param>
    /// <param name="overlap">Overlap даёт кускам немного общего контекста, чтобы жестко не
    /// резать мысль на границе чанков</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public IReadOnlyCollection<DocumentChunk> Chunk(
        string documentName,
        string text,
        int chunkSize = 500,
        int overlap = 100)
    {
        if (string.IsNullOrWhiteSpace(text))
            return [];
        
        if(chunkSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(chunkSize));

        if (overlap < 0 || overlap >= chunkSize)
            throw new ArgumentOutOfRangeException(nameof(overlap));

        var chunks = new List<DocumentChunk>();
        var start = 0;
        var index = 0;

        while (start < text.Length)
        {
            var length = Math.Min(chunkSize, text.Length - start);
            var content = text.Substring(start, length).Trim();

            if (!string.IsNullOrWhiteSpace(content))
            {
                chunks.Add(new DocumentChunk
                {
                    DocumentName = documentName,
                    Content = content,
                    ChunkIndex = index++
                });
            }
            
            start += chunkSize - overlap;
        }

        return chunks;
    }
}