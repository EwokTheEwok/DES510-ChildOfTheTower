using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBlockUI : MonoBehaviour {
    [Serializable]
    private class BlockTile
    {
        public GameObject full;
        public GameObject empty;

        public void SetActive(bool active)
        {
            empty.SetActive(active);
            full.SetActive(false);
        }

        public void Fill(bool fill)
        {
            full.SetActive(fill);
        }
    }

    [Header("Prefabs")]
    public GameObject _FullBlockTilePrefab;
    public GameObject _EmptyBlockTilePrefab;
    public GameObject _BlockUIPrefab;

    [Header("Settings")]
    public int _MaxBlockTiles = 20;
    public int _PixelSpacing = 5;
    public int _TileSize = 32;
    public int _ActiveTileCount = 1;
    public float _RecoverFadeTime = 2f;
    bool _RestartFade = false;
    IEnumerator _FadeCoroutine;
    BlockTile[] _BlockTileList;

    GameObject _UIObject;
    GameObject _BlockTilePanel;
    [HideInInspector] public GameObject playerPanel;
    [HideInInspector] public GameObject crossPanel;
    [HideInInspector] public GameObject arrowPanel;
    [HideInInspector] public GameObject placePanel;
    [HideInInspector] public GameObject recoverPanel;
    [HideInInspector] public RectTransform blockUIUp;
    [HideInInspector] public RectTransform blockUIDown;
    [HideInInspector] public RectTransform blockUILeft;
    [HideInInspector] public RectTransform blockUIRight;

    private void Awake()
    {
        _UIObject = Instantiate(_BlockUIPrefab);
        Transform uiTransform = _UIObject.transform;
        arrowPanel = uiTransform.Find("Arrow Panel").gameObject;
        crossPanel = uiTransform.Find("Cross").gameObject;
        playerPanel = uiTransform.Find("Player Panel").gameObject;
        placePanel = uiTransform.Find("Place").gameObject;
        recoverPanel = uiTransform.Find("Recover").gameObject;
        _BlockTilePanel = uiTransform.Find("Player Panel/Block Tile Panel").gameObject;

        blockUIUp = uiTransform.Find("Arrow Panel/Up").gameObject.transform as RectTransform;
        blockUIDown = uiTransform.Find("Arrow Panel/Down").gameObject.transform as RectTransform;
        blockUILeft = uiTransform.Find("Arrow Panel/Left").gameObject.transform as RectTransform;
        blockUIRight = uiTransform.Find("Arrow Panel/Right").gameObject.transform as RectTransform;

        _BlockTileList = new BlockTile[_MaxBlockTiles];
        for (int i = 0; i < _BlockTileList.Length; i++)
        {
            BlockTile tile = new();
            _BlockTileList[i] = tile;
            tile.empty = Instantiate(_EmptyBlockTilePrefab, _BlockTilePanel.transform);
            tile.full = Instantiate(_FullBlockTilePrefab, _BlockTilePanel.transform);
            RectTransform fullTransform = tile.full.transform as RectTransform;
            RectTransform emptyTransform = tile.empty.transform as RectTransform;
            fullTransform.localPosition = new Vector3((i * _TileSize) + (_PixelSpacing * i), 0f, 0f);
            emptyTransform.localPosition = new Vector3((i * _TileSize) + (_PixelSpacing * i), 0f, 0f);
            if (i < _ActiveTileCount)
                _BlockTileList[i].SetActive(true);
        }
    }

    void Start()
    {
        Vector3 localPos = _BlockTilePanel.transform.localPosition;
        _BlockTilePanel.transform.localPosition = new Vector3(-((_ActiveTileCount - 1f) * _TileSize * 0.5f) - (_PixelSpacing * 0.5f * (_ActiveTileCount - 1f)), localPos.y, localPos.z);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetTile(int index, bool full)
    {
        if (index < _MaxBlockTiles)
            _BlockTileList[index].Fill(full);
    }

    public void EmptyTiles()
    {
        foreach (BlockTile tile in _BlockTileList)
        {
            tile.Fill(false);
        }
    }

    public void SetLimit(int count)
    {
        _ActiveTileCount = count;

        for (int i = 0; i < _BlockTileList.Length; i++)
        {
            if (i < _ActiveTileCount)
                _BlockTileList[i].SetActive(true);
            else
                _BlockTileList[i].SetActive(false);
        }
        Vector3 localPos = _BlockTilePanel.transform.localPosition;
        _BlockTilePanel.transform.localPosition = new Vector3(-((_ActiveTileCount - 1f) * _TileSize * 0.5f) - (_PixelSpacing * 0.5f * (_ActiveTileCount - 1f)), localPos.y, localPos.z);
    }

    public void SetPanelsActive(bool active)
    {
        crossPanel.SetActive(active);
        arrowPanel.SetActive(active);
        playerPanel.SetActive(active);
    }

    public void StartFade()
    {
        if (_FadeCoroutine != null)
            StopCoroutine(_FadeCoroutine);
        _FadeCoroutine = FadeRecover();
        StartCoroutine(_FadeCoroutine);
    }

    private IEnumerator FadeRecover()
    {
        Image recoverImage = recoverPanel.GetComponent<Image>();
        recoverImage.color = Color.white;
        yield return new WaitForSeconds(2f);
        for (float fadeFraction = 0; fadeFraction < 1; fadeFraction += Time.deltaTime / _RecoverFadeTime)
        {
            Color newColor = recoverImage.color;
            newColor.a = Mathf.Lerp(1f, 0f, fadeFraction);
            recoverImage.color = newColor;
            yield return null;
        }
    }

    public void SetColour(Color color)
    {
        foreach (BlockTile tile in _BlockTileList)
        {
            tile.empty.GetComponent<Image>().color = color;
            tile.full.GetComponent<Image>().color = color;
        }
    }
}
