using TMPro;
using UnityEngine;

public class FontDictionaryFixer : MonoBehaviour
{
    void Start()
    {
        // Ждём конца кадра, чтобы все TMP-компоненты успели инициализироваться
        StartCoroutine(FixFontsNextFrame());
    }
    
    System.Collections.IEnumerator FixFontsNextFrame()
    {
        yield return null; // Ждём один кадр
        
        TMP_FontAsset[] allFonts = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();
        foreach (TMP_FontAsset font in allFonts)
        {
            if (font == null) continue;
            
            // Принудительная очистка кеша кернинга
            var field = typeof(TMP_FontAsset).GetField("m_FontFeatureTable", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (field != null)
            {
                var table = field.GetValue(font);
                var lookupField = table.GetType().GetField("m_GlyphPairAdjustmentRecordLookup",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    
                if (lookupField != null)
                {
                    // Создаём новый чистый словарь
                    lookupField.SetValue(table, 
                        new System.Collections.Generic.Dictionary<uint, 
                            UnityEngine.TextCore.LowLevel.GlyphPairAdjustmentRecord>());
                }
            }
            
            // Перечитываем определение шрифта
            font.ReadFontAssetDefinition();
        }
        
        Debug.Log($"Инициализировано шрифтов: {allFonts.Length}");
    }
}