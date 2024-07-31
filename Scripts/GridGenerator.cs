using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour {

    [SerializeField] private GameObject gridNumberObject;
    private int startPositionX = -750;
    private int startPositionY = 300;
    private int offset = 75;
    private int offsetX = 0;
    private int offsetY = 0;


    public GridPosition[,] InstantiateGridNumbers(int gridCount, Transform parent) {
        GridPosition[,] gridToReturn = new GridPosition[gridCount,gridCount];
        int gridToReturnIndex = 0;
        for (int y = 0; y < gridCount; y++) {
            for (int x = 0; x < gridCount; x++) {
                offsetX = 5*(x/3);
                offsetY = 5*(y/3);

                GameObject currentGridNumberObject = Instantiate(gridNumberObject, parent);
                currentGridNumberObject.transform.localPosition = new Vector3(startPositionX + (x * offset)+offsetX, startPositionY - (y * offset)-offsetY, 0);
                gridToReturn[y,x] = currentGridNumberObject.GetComponent<GridPosition>();
                gridToReturn[y, x].rowColNum.row = y;
                gridToReturn[y, x].rowColNum.col = x;
                gridToReturnIndex++;
            }
        }
        return gridToReturn;
    }
}

