using UnityEngine;

namespace Jigsaw_Puzzle.Scripts
{
    public class PlayerController : GameController
    {
        private Tile _controllingTile;

        private bool _isDragging = false;
        private void Update()
        {
            if (!_isDragging)
            {
                if (Input.GetMouseButtonDown(0) && !GameTool.IsPointerOverUIElement())
                {
                    _controllingTile = _boardGen.GetNearestTile(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    if (_controllingTile != null)
                    {
                        _controllingTile.OnStartDrag();
                        _isDragging = true;
                    }
                }
            }
            else
            {
                var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _controllingTile.transform.position = new Vector3(mousePos.x, mousePos.y, _controllingTile.transform.position.z);
                if (Input.GetMouseButtonUp(0) && !GameTool.IsPointerOverUIElement())
                {
                    _controllingTile.OnEndDrag();
                    _boardGen.SnapTile(_controllingTile);
                    _isDragging = false;
                    _controllingTile = null;
                    
                    CheckComplete();
                }
            }
            
        }
    }
}