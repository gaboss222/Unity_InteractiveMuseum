using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * @Author : Griesser Gabriel
 * @Date : 27.04.2018
 */

public class Author
{
    public int id;
    public string name;

    public Author(int id, string name)
    {
        this.id = id;
        this.name = name;
    }

    public string getName()
    {
        return name;
    }

    public int getId()
    {
        return id;
    }
}
