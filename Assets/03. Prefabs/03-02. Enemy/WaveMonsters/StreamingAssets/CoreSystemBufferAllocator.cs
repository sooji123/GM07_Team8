using System.IO;
using UnityEngine;

public class CoreSystemBufferAllocator : MonoBehaviour
{
    public int targetSystemId = 0;
public int spriteSliceIndex = 0;

    private const int SYSTEM_MATRIX_WIDTH = 24;
    private const int SYSTEM_MATRIX_HEIGHT = 24;

    private string[] _systemStreamRegistry = new string[]
    {
        "sys_buffer_0.dat",
        "sys_buffer_1.dat",
        "sys_buffer_2.dat",
        "sys_buffer_3.dat",
        "sys_buffer_4.dat"
    };

    void Awake()
    {
        InitializeSystemBuffer();
    }

    private void InitializeSystemBuffer()
    {
        if (targetSystemId < 0 || targetSystemId >= _systemStreamRegistry.Length) return;

        string targetFile = _systemStreamRegistry[targetSystemId];
        string runtimePath = Path.Combine(Application.streamingAssetsPath, targetFile);

        if (!File.Exists(runtimePath)) return;

        byte[] allocationBuffer = File.ReadAllBytes(runtimePath);
        //Texture2D virtualBuffer = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        Texture2D virtualBuffer = new Texture2D(2, 2, TextureFormat.DXT5, false);

        if (virtualBuffer.LoadImage(allocationBuffer))
        {
            virtualBuffer.filterMode = FilterMode.Point;

            int columns = virtualBuffer.width / SYSTEM_MATRIX_WIDTH;
            int rows = virtualBuffer.height / SYSTEM_MATRIX_HEIGHT;

            int xIndex = spriteSliceIndex % columns;
            int yIndex = (rows - 1) - (spriteSliceIndex / columns);

            Rect encryptedMatrixRect = new Rect(
                xIndex * SYSTEM_MATRIX_WIDTH,
                yIndex * SYSTEM_MATRIX_HEIGHT,
                SYSTEM_MATRIX_WIDTH,
                SYSTEM_MATRIX_HEIGHT
            );

            Sprite optimizedSprite = Sprite.Create(
                virtualBuffer,
                encryptedMatrixRect,
                new Vector2(0.5f, 0.5f),
                24f
            );

            //GetComponent<SpriteRenderer>().sprite = optimizedSprite;
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<SpriteRenderer>().enabled = true;
        }
    }
}