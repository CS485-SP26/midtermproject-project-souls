using UnityEngine;
using Farming;
using Unity.VisualScripting;
using UnityEngine.Tilemaps;
using NUnit.Framework;

namespace Character
{
    public class RaydownSelector : TileSelector //this selector will detect tiles under the player!
    {
            public float rayCastDistance; //set how far down the farm tile will be detected under the player
            RaycastHit hitInfo;

            void Update()
            {
                if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hitInfo, rayCastDistance)) //remember goto definition for parameters! 
                {
                if (hitInfo.collider.TryGetComponent<FarmTile>(out FarmTile tile)) //note: 'out' declares a new element, so you dont need to set one to reference under the public class
                {
                    SetActiveTile(tile);
                    if (!currentSelection.Contains(tile))
                    { currentSelection.Add(tile); }
                }
                else
                {
                    SetActiveTile(null);
                    currentSelection.Clear();
                }
                }
            }
    }
}