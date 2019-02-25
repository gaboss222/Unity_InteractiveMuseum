using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.IO;


/*
 * @Author : Griesser Gabriel
 * @Date : 27.04.2018
 */

public class JsonReader
{

    public string fileName;
    JSONNode jsonNode;
    List<Author> _authors;
    List<Paint> _paints;
    List<Room> _rooms;

    /*
     * Constructor
     */
    public JsonReader(string fileName)
    {
        this.fileName = fileName;
        string str = Read();

        jsonNode = JSON.Parse(str);

        _authors = new List<Author>();
        _paints = new List<Paint>();
        _rooms = new List<Room>();

        FillList();

        InitializingRoom();
    }


    /*
     * Read data from file 
     */
    string Read()
    {
        StreamReader sr = new StreamReader(Application.dataPath + "/" + fileName);
        string content = sr.ReadToEnd();
        sr.Close();

        return content;
    }


    /*
     * Fill lists with data from Json file
     */
    void FillList()
    {
        //Fill author list
        for (int i = 0; i < jsonNode["authors"].Count; i++)
        {
            int id = int.Parse(jsonNode["authors"][i]["id"].Value);
            string name = jsonNode["authors"][i]["name"].Value;
            _authors.Add(new Author(id, name));
        }

        //Fill picture list
        for (int i = 0; i < jsonNode["pictures"].Count; i++)
        {
            int authorID = int.Parse(jsonNode["pictures"][i]["author"].Value);
            string path = Application.dataPath + "/"  + jsonNode["pictures"][i]["path"].Value;
            string title = jsonNode["pictures"][i]["title"].Value;
            int price = int.Parse(jsonNode["pictures"][i]["price"].Value);
            int width = int.Parse(jsonNode["pictures"][i]["x"].Value);
            int height = int.Parse(jsonNode["pictures"][i]["y"].Value);

            _paints.Add(new Paint(path, title, authorID, price, width, height));
        }

        //Replace the ID with the name
        foreach (Paint p in _paints)
        {
            foreach (Author a in _authors)
            {
                if (p.authorId == a.id)
                {
                    p.author = a.name;
                }
            }
        }        
    }

    /*
     * Create a room for each author and fill it with corresponding paint
     */
    void InitializingRoom()
    {

        foreach (Author a in _authors)
        {
            _rooms.Add(new Room(a));
        }

        foreach(Room r in _rooms)
        {
            foreach(Paint p in _paints)
            {
                if(p.authorId == r.GetIdAuthor())
                {
                    r.AddPicture(p);
                }
            }
        }
    }

    /*
     * Getter
     */
    public List<Room> GetRoomList()
    {
        return _rooms;
    }
}