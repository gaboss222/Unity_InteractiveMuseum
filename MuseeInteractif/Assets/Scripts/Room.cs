using System.Collections.Generic;
using System.IO;
using UnityEngine;

/*
 * @Author : Griesser Gabriel
 * @Date : 01.05.2018
 */

/*
 * This class represent a room
 */
public class Room
{

    const float MARGIN = 500;
    const float DIVIDEWALL = 1000;
    const float DIVIDEPAINT = 1500;

    Author author;
    public List<Paint> _paints;

    GameObject prefabShadowRoom, prefabShadowPaint;
    Transform wallLeft, wallRight, wallBottom;

    float widthWallLeft, widthWallRight, widthWallBottom, widthWallLeftRight;
    float sumWidthPaintLeft   = 0;
    float sumWidthPaintRight  = 0;
    float sumWidthPaintBottom = 0;

    /*
     * Constructor
     */
    public Room(Author a)
    {
        _paints = new List<Paint>();
        author = a;
    }


    /*
     * We recover room and paintings prefab
     */
    public void SetPrefab(GameObject prefabRoom, GameObject prefabPaint)
    {
        this.prefabShadowRoom = prefabRoom;
        this.prefabShadowPaint = prefabPaint;
    }

    /*
     * Initializing room
     */
    public void InitializingRoom()
    {
        //Sort paints the largest to the smallest
        _paints.Sort((x, y) => x.width.CompareTo(y.width)*-1);

        //Get wall
        wallLeft = prefabShadowRoom.transform.Find("WallLeft");
        wallRight = prefabShadowRoom.transform.Find("WallRight");
        wallBottom = prefabShadowRoom.transform.Find("WallBottom");
        
        CalculateWidthWall();
    }

    /*
     * Distribute paints on the 3 walls
     * Calculate width for each wall
     */
    void CalculateWidthWall()
    {
        int numWall = 1;
        bool isBottomFull = false;

        //If the largest painting is at least 1.5x larger than the one below, place it on the back wall ("pretty" effect)
        if (_paints[0].width > 1.5 * _paints[1].width)
        {
            numWall = 2;
            isBottomFull = true;
        }


        //Distribute and save sum
        {
            foreach (Paint paint in _paints)
            {
                float widthPaint = (float)paint.width;

                switch(numWall)
                {
                    //left wall
                    case 1:
                        paint.wall = wallLeft;

                        widthWallLeft += widthPaint + MARGIN;
                        sumWidthPaintLeft += widthPaint;

                        numWall = (isBottomFull) ? 3 : 2;
                        break;

                    //bottom wall
                    case 2:
                        paint.wall = wallBottom;

                        widthWallBottom += widthPaint + MARGIN;
                        sumWidthPaintBottom += widthPaint;

                        numWall = 3;
                        break;

                    //right wall
                    case 3:
                        paint.wall = wallRight;

                        widthWallRight += widthPaint + MARGIN;
                        sumWidthPaintRight += widthPaint;

                        numWall = 1;
                        break;
                }
            }
        }


        //"Pretty" room
        { 
            //The size of the right/left wall is the largest of the two sizes
            if (widthWallRight > widthWallLeft)
            {
                widthWallLeftRight = widthWallRight;
            }
            else
            {
                widthWallLeftRight = widthWallLeft;
            }

            //To avoid disproportion
            if (widthWallLeftRight > 4 * widthWallBottom)
            {
                float sizeMin = widthWallLeftRight / widthWallBottom;
                widthWallBottom = (sizeMin / 2) * widthWallBottom;
            }
            else if (widthWallBottom > 4 * widthWallLeftRight)
            {
                float sizeMin = widthWallBottom / widthWallLeftRight;
                widthWallLeftRight = (sizeMin / 2) * widthWallLeftRight;
            }
        }
    }

    /*
     * Create a paint GameObject (with texture) for paint passed in parameter
     * Set paint position in the center of the wall
     */
    public void AddPaint(Paint p, Transform wall)
    {

        float widthPaint = (float)p.width;
        float heightPaint = (float)p.height;

        GameObject paint = GameObject.Instantiate(prefabShadowPaint);
        Transform picture = paint.transform.GetChild(0);

        paint.name = p.title;
        SetMaterial(p, picture);

        paint.transform.localScale = new Vector3(widthPaint / DIVIDEPAINT, heightPaint / DIVIDEPAINT, 0.07f);
        paint.transform.localPosition = wall.transform.position;


        switch(wall.name)
        {
            case "WallBottom":
                paint.transform.Rotate(0, 90f, 0);
                paint.transform.SetParent(wall, true);

                paint.transform.localPosition += new Vector3(0, 0, -0.5f);
                paint.transform.Rotate(0, 180f, 0);
                break;

            case "WallLeft":
                paint.transform.SetParent(wall, true);

                paint.transform.localPosition += new Vector3(0, 0, -0.5f);
                paint.transform.Rotate(0, 180f, 0);
                break;

            case "WallRight":
                paint.transform.SetParent(wall, true);

                paint.transform.localPosition += new Vector3(0, 0, 0.5f);
                break;

        }
    }

    /*
     * Calculate space between each paint
     * And place them 
     */
    public void PaintSpacing(Transform wall)
    {
        int nbPaint = wall.childCount;
        float sumWidthPaint = 0;
        float widthWall = 0;
        float space = 0;
        float myPos = 0;

        //Align to the left
        PosPaintLeft(wall);

        // Calculate space between each paintings
        {
            switch(wall.name)
            {
                case "WallLeft":
                    sumWidthPaint = sumWidthPaintLeft;
                    break;

                case "WallRight":
                    sumWidthPaint = sumWidthPaintRight;
                    break;

                case "WallBottom":
                    sumWidthPaint = sumWidthPaintBottom;
                    break;
            }

            widthWall = (wall == wallLeft || wall == wallRight) ? widthWallLeftRight : widthWallBottom;

            // space = (width current wall - sum width paint on this wall) / number of paint + 1
            // _space_ PAINT _space_ PAINT _space_ PAINT _space_
            space = (widthWall - sumWidthPaint) / (nbPaint + 1);
            space = space / widthWall;
        }

        //Place the paintings
        {
            Transform paint;
            for (int i = 0; i < nbPaint; i++)
            {
                paint = wall.GetChild(i);

                myPos += (i == 0) ? space : space + paint.transform.localScale.x / 2;

                switch (wall.name)
                {
                    case "WallLeft":
                        paint.localPosition += new Vector3(myPos, 0, 0);
                        break;

                    case "WallRight":
                        paint.localPosition += new Vector3(-myPos, 0, 0);
                        break;

                    case "WallBottom":
                        if (nbPaint == 1)
                            paint.localPosition += new Vector3(-paint.localPosition.x, 0, 0);
                        else
                            paint.localPosition += new Vector3(myPos, 0, 0);
                        break;
                }
                

                myPos += paint.transform.localScale.x;
            }
        }
    }

    /*
     * Align all the paints to the left edge of the wall
     */
    void PosPaintLeft(Transform wall)
    {
        int nbPaint = wall.childCount;
        float myPos = 0;

        for (int i = 0; i < nbPaint; i++)
        {
            Transform paint = wall.GetChild(i);
            myPos = 0.5f - (paint.localScale.x / 2) - 0.03f;

            switch(wall.name)
            {
                case "WallLeft":
                    paint.localPosition += new Vector3(-myPos, 0, 0);
                    break;

                case "WallRight":
                    paint.localPosition += new Vector3(myPos, 0, 0);
                    break;

                case "WallBottom":
                    paint.localPosition += new Vector3(-myPos, 0, 0);
                    break;
            }
        }
    }

    /*
     * Set paint texture (texture = picture)
     */
    void SetMaterial(Paint p, Transform picture)
    {
        Texture2D textPaint = null;
        byte[] fileData;

        fileData = File.ReadAllBytes(p.path);
        textPaint = new Texture2D(2, 2);
        textPaint.LoadImage(fileData);
      
        picture.GetComponent<Renderer>().material.mainTexture = textPaint;
    }

    /*
     * Add a paint
     */
    public void AddPicture(Paint p)
    {
        _paints.Add(p);
    }

    /*
     * Getter
     */
    public float GetWallLeftRightSize()
    {
        return widthWallLeftRight / DIVIDEWALL;
    }
    public float GetWallBottomSize()
    {
        return widthWallBottom / DIVIDEWALL;
    }
    public int GetIdAuthor()
    {
        return author.getId();
    }
    public string GetNameAuthor()
    {
        return author.getName();
    }

    /*
     * Height of room = max height of the highest paint
     */
    public float GetMaxHeight()
    {
        float temp = 0;
        foreach (Paint p in _paints)
        {
            if (p.height > temp)
            {
                temp = p.height;
            }
        }
        return temp / DIVIDEWALL;
    }

    
}
