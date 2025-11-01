using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public Vector2Int gridPosition;
    public SpriteRenderer spriteRenderer;
    public Sprite XSprite;
    public Sprite OSprite;

    private GameManager gameManager;

    void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    public void Initialize(Vector2Int position, GameManager manager)
    {
        gridPosition = position;
        gameManager = manager;
        SetSymbol(0);
    }

    private void OnMouseDown()
    {
        gameManager.PlayerMove(gridPosition);
    }

    public void SetSymbol(int val)
    {
        // val: 0 empty, 1 player (X), -1 ai (O)
        if (spriteRenderer == null) return;

        if (val == 1)
        {
            spriteRenderer.sprite = XSprite;
        }
        else if (val == -1)
        {
            spriteRenderer.sprite = OSprite;
        }
        else
        {
            spriteRenderer.sprite = null;
        }
    }
}
