namespace AiRagDemo.Infrastructure.VectorSearch;

public static class VectorMath
{
    /// <summary>
    /// Косинусное сходство двух векторов a и b (мера близости двух векторов,
    /// определяющая косинус угла между ними в многомерном пространстве)
    /// </summary>
    /// <remarks>
    /// Интерпретация результата:
    /// 1 — направления полностью совпадают,
    /// 0 — ортогональны (нет сходства по направлению),
    /// -1 — противоположные направления.
    /// Для эмбеддингов обычно ожидают значение ближе к 1 для похожих текстов.
    /// </remarks>>
    public static double CosinesSimilarity(float[] a, float[] b)
    {
        // Векторы должны быть одной размерности, иначе сравнение некорректно.
        if (a.Length != b.Length)
            throw new ArgumentException("Vectors must be of the same length.");

        double dot = 0; // сумма попарных произведений (a[i]*b[i])
        double normA = 0; // сумма квадратов компонентов a
        double normB = 0; // сумма квадратов компонентов b

        for (int i = 0; i < a.Length; i++)
        {
            dot += a[i] * b[i];
            normA += a[i] * a[i];
            normB += b[i] * b[i];
        }
        
        if(normA == 0 || normB == 0)
            return 0;
        
        return dot / (Math.Sqrt(normA) * Math.Sqrt(normB));
    }
}