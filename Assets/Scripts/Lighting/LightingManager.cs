using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class LightingManager : MonoBehaviour
{
    public Tilemap wallTilemap;
    public Tilemap floorTilemap;
    public Tilemap mistTilemap;
    public List<Character> characters; 
    public LayerMask layerMask;
    public LayerMask mistLayerMask;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < characters.Count; i++) {
            characters[i].OnMove.AddListener(OnCharacterMove);
        }   
    }

    // Update is called once per frame
    void Update()
    {
    }

    public int GetDarknessLevel(GameObject tile, Character character, Vector3 characterPos) {
        var dist = Vector3.Distance(tile.transform.position, characterPos);
        if (dist <= character.minSightRange) {
            return 0;
        }
        if (dist <= character.midSightRange) {
            return 1;
        }

        return 2;
    }

    public void OnCharacterMove(Character character, Vector3 position) {
        UpdateLighting(character, position);
    }
    void UpdateLighting(Character character, Vector3 position) {
        UpdateWallShadows(character, position);
        UpdateFloorShadows(character, position);
        UpdateMistShadows(character, position);
    }

    void UpdateWallShadows(Character character, Vector3 characterPos) {
        for (int x = wallTilemap.cellBounds.min.x; x < wallTilemap.cellBounds.max.x; x++) {
            for (int y = wallTilemap.cellBounds.min.y; y < wallTilemap.cellBounds.max.y; y++) {
                var tilePos = new Vector3Int(x, y, 0);
                var wallTileObject = wallTilemap.GetInstantiatedObject(tilePos);

                if (wallTileObject) {
                    wallTileObject.GetComponent<ShadowTile>().SetDarkness(3);
                    var charPointer =  wallTileObject.transform.position - characterPos;
                    var distanceToChar = charPointer.magnitude;
                    if (distanceToChar < character.maxSightRange) {
                        
                        var hit = Physics2D.Raycast(characterPos, charPointer, 50, layerMask);
                        if (hit) {
                            var circleHits = Physics2D.CircleCastAll(hit.collider.transform.position, 0.51f, Vector2.zero);
                            for (int j = 0; j < circleHits.Length; j++) {
                                var circleHit = circleHits[j];
                                var wallTileNearby = circleHit.collider.gameObject.GetComponent<ShadowTile>();
                                if (wallTileNearby) {
                                    wallTileNearby.SetDarkness(GetDarknessLevel(wallTileNearby.gameObject, character, characterPos));
                                }
                            }
                            var wallTileComp = hit.collider.gameObject.GetComponent<ShadowTile>();
                            if (wallTileComp) {
                                wallTileComp.SetDarkness(GetDarknessLevel(wallTileComp.gameObject, character, characterPos));
                            }
                        }
                    }
                }
            }
        }
    }

    void UpdateFloorShadows(Character character, Vector3 characterPos) {
        for (int x = floorTilemap.cellBounds.min.x; x < floorTilemap.cellBounds.max.x; x++) {
            for (int y = floorTilemap.cellBounds.min.y; y < floorTilemap.cellBounds.max.y; y++) {
                var tilePos = new Vector3Int(x, y, 0);
                var floorTileObject = floorTilemap.GetInstantiatedObject(tilePos);

                if (floorTileObject) {
                    var floorShadowTile = floorTileObject.GetComponent<ShadowTile>();
                    floorShadowTile.SetDarkness(3);
                    var charPointer =  floorTileObject.transform.position - characterPos;
                    var distanceToChar = charPointer.magnitude;
                    if (distanceToChar < character.maxSightRange) {
                        var hit = Physics2D.Linecast(floorTileObject.transform.position, characterPos, layerMask);
                        if (!hit || hit.fraction <= 0) {
                            floorShadowTile.SetDarkness(GetDarknessLevel(floorTileObject, character, characterPos));
                        }
                    }
                }
            }
        }
    }

    void UpdateMistShadows(Character character, Vector3 characterPos) {
        for (int x = mistTilemap.cellBounds.min.x; x < mistTilemap.cellBounds.max.x; x++) {
            for (int y = mistTilemap.cellBounds.min.y; y < mistTilemap.cellBounds.max.y; y++) {
                var tilePos = new Vector3Int(x, y, 0);
                var mistTileObject = mistTilemap.GetInstantiatedObject(tilePos);

                if (mistTileObject) {
                    var mistShadowTile = mistTileObject.GetComponent<ShadowTile>();
                    var boardTile = mistTileObject.GetComponent<BoardVisualTile>();

                    if (!boardTile.GetRevealed()) {
                        mistShadowTile.SetDarkness(3);
                        var charPointer =  mistTileObject.transform.position - characterPos;
                        var distanceToChar = charPointer.magnitude;
                        if (distanceToChar < character.maxSightRange) {
                            mistShadowTile.SetDarkness(GetDarknessLevel(mistTileObject, character, characterPos));
                        }
                    }
                    else {
                        mistShadowTile.SetDarkness(0);
                    }

                }
            }
        }
    }

}
