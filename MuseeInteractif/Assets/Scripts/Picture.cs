using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * @Author : Griesser Gabriel
 * @Date : 27.04.2018
 */

public class Paint
{
    public int id, width, height, authorId, price;
    public string author, title, path;
    public Transform wall;

    public Paint(string path, string title, int authorId, int price, int x, int y)
    {
        this.path = path;
        this.title = title;
        this.authorId = authorId;
        this.price = price;
        this.width = x;
        this.height = y;
    }
}
