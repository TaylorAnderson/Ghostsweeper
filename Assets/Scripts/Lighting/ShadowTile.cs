using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowTile : MonoBehaviour
{
    public List<GameObject> darkness;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void SetDarkness(int dark) {
        for (int i = 0; i < darkness.Count; i++) {
            darkness[i].SetActive(false);
        }
        if (dark > 0 && dark < darkness.Count+1) {
            darkness[dark - 1].SetActive(true);
        }
    }
}
