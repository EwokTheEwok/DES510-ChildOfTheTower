using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class BrickBehaviorTest : MonoBehaviour
{
    // Brick Prefab
    [SerializeField] private GameObject _brickPrefab;

    public GameObject _currentBrick;

    private Transform[] _spawnLocations;

    private int _spawnCount;

    public int _spawnLimit;

    [SerializeField] private LayerMask _blockMask;

    public BrickAnimHandler AnimationHandler;

    // Depending on which key is pressed, the corresponding spawn location is mapped and a new brick is instantiated (if not blocked by a previous brick)
    void BrickExtend()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame && _spawnCount < _spawnLimit)
        {
            Transform sp = _spawnLocations[0];

            if (!IsBlocked(sp))
            {
                _currentBrick = Instantiate(_brickPrefab, sp.position, sp.rotation);
                AnimationHandler.AnimationHandler();
                _spawnCount++;
                CacheSpawnLocations();
            }
        }

        else if (Keyboard.current.leftArrowKey.wasPressedThisFrame && _spawnCount < _spawnLimit)
        {
            Transform sp = _spawnLocations[1];

            if (!IsBlocked(sp))
            {
                _currentBrick = Instantiate(_brickPrefab, sp.position, sp.rotation);
                AnimationHandler.AnimationHandler();
                _spawnCount++;
                CacheSpawnLocations();
            }
        }

        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame && _spawnCount < _spawnLimit)
        {
            Transform sp = _spawnLocations[2];

            if (!IsBlocked(sp))
            {
                _currentBrick = Instantiate(_brickPrefab, sp.position, sp.rotation);
                AnimationHandler.AnimationHandler();
                _spawnCount++;
                CacheSpawnLocations();
            }
        }

        else if (Keyboard.current.ctrlKey.wasPressedThisFrame && _spawnCount < _spawnLimit)
        {
            Transform sp = _spawnLocations[3];

            if (!IsBlocked(sp))
            {
                _currentBrick = Instantiate(_brickPrefab, sp.position, sp.rotation);
                AnimationHandler.AnimationHandler();
                _spawnCount++;
                CacheSpawnLocations();
            }
        }

        else if (Keyboard.current.upArrowKey.wasPressedThisFrame && _spawnCount < _spawnLimit)
        {
            Transform sp = _spawnLocations[4];

            if (!IsBlocked(sp))
            {
                _currentBrick = Instantiate(_brickPrefab, sp.position, sp.rotation);
                AnimationHandler.AnimationHandler();
                _spawnCount++;
                CacheSpawnLocations();
            }
        }

        else if (Keyboard.current.downArrowKey.wasPressedThisFrame && _spawnCount < _spawnLimit)
        {
            Transform sp = _spawnLocations[5];

            if (!IsBlocked(sp))
            {
                _currentBrick = Instantiate(_brickPrefab, sp.position, sp.rotation);
                AnimationHandler.AnimationHandler();
                _spawnCount++;
                CacheSpawnLocations();
            }
        }
    }

    // Bool which determines whether a brick is blocked by using a CheckBox
    private bool IsBlocked(Transform sp)
    {
        Vector3 halfExtents = new Vector3(0.3f, 0.3f, 0.3f);
        return Physics.CheckBox(sp.position, halfExtents, sp.rotation, _blockMask);
    }

    // After a new brick is spawned, updates the transforms used for collision detection to be relative to the new brick
    void CacheSpawnLocations()
    {
        _spawnLocations = new Transform[_currentBrick.transform.childCount];
        for (int i = 0; i < _spawnLocations.Length; i++)
            _spawnLocations[i] = _currentBrick.transform.GetChild(i);

        Debug.Log(_spawnCount);
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //_spawnLimit = 100;

        //AnimationHandler.AnimationHandler();

        CacheSpawnLocations();
    }

    // Update is called once per frame
    void Update()
    {
        BrickExtend();
    }
}
