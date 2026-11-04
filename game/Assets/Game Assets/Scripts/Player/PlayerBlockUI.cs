using System;
using UnityEngine;

public class PlayerBlockUI : MonoBehaviour {
    [Serializable]
    private class BlockTile
    {
        public GameObject full;
        public GameObject empty;

        public void SetActive(bool active)
        {
            empty.SetActive(active);
            full.SetActive(active);
        }
    }

    public GameObject _FullBlockTilePrefab;
    public GameObject _EmptyBlockTilePrefab;

    public int _MaxBlockTiles = 20;
    public int _PixelSpacing = 5;
    public int _TileSize = 32;
    public int _ActiveTileCount = 1;
    BlockTile[] _BlockTileList;
    public static int test = 5;


    GameObject _BlockTilePanel;
    /*
    GameObject _CrossUI;
    GameObject _ArrowUI;
    RectTransform _BlockUIUp;
    RectTransform _BlockUIDown;
    RectTransform _BlockUILeft;
    RectTransform _BlockUIRight;
    */

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _BlockTilePanel = transform.Find("Player Panel/Block Tile Panel").gameObject;
        _BlockTileList = new BlockTile[_MaxBlockTiles];
        for (int i = 0; i < _BlockTileList.Length; i++)
        {
            BlockTile tile = new ();
            _BlockTileList[i] = tile;
            tile.empty = Instantiate(_EmptyBlockTilePrefab, _BlockTilePanel.transform);
            tile.full = Instantiate(_FullBlockTilePrefab, _BlockTilePanel.transform);
            RectTransform fullTransform = tile.full.transform as RectTransform;
            RectTransform emptyTransform = tile.empty.transform as RectTransform;
            fullTransform.localPosition = new Vector3((i * _TileSize) + (_PixelSpacing * 0.5f * i), 0f, 0f);
            emptyTransform.localPosition = new Vector3((i * _TileSize) + (_PixelSpacing * 0.5f * i), 0f, 0f);
            if (i < _ActiveTileCount)
                _BlockTileList[i].SetActive(true);
        }
        _BlockTilePanel.transform.localPosition = new Vector3(-((_ActiveTileCount - 1f) * _TileSize * 0.5f) - (_PixelSpacing * 0.5f * (_ActiveTileCount - 1f)), 0f, 0f);

        /*
        _ArrowUI = transform.Find("Arrow Panel").gameObject;
        _CrossUI = transform.Find("Cross").gameObject;

        _BlockUIUp = transform.Find("Arrow Panel/Up").gameObject.transform as RectTransform;
        _BlockUIDown = transform.Find("Arrow Panel/Down").gameObject.transform as RectTransform;
        _BlockUILeft = transform.Find("Arrow Panel/Left").gameObject.transform as RectTransform;
        _BlockUIRight = transform.Find("Arrow Panel/Right").gameObject.transform as RectTransform;
        */


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetActiveCount(int count)
    {
        _ActiveTileCount = count;
        
    }
}
