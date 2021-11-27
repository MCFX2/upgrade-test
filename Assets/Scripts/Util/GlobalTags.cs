//Author(s): Charles Osberg
//File: GlobalTags.cs
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// System for managing multitags.
/// </summary>
public static class GlobalTags
{
    static GlobalTags()
    {
        SceneManager.sceneUnloaded += SceneManagerOnSceneUnloaded;
        LoadTagCache();
    }

    //Wipe object and tag caches on scene load/reload
    private static void SceneManagerOnSceneUnloaded(Scene scene)
    {
        objectStore.Clear();
        tagStore.Clear();
    }

    /// <summary>
    /// Class abstracting away ugly tag validation stuff
    /// </summary>
    private static class TagValidator
    {
        /// <summary>
        /// Cleans up the tag list, making it valid for the tag system.
        /// For debug purposes. Does nothing during a real game.
        /// </summary>
        /// <param name="tags">List of tags to clean.</param>
        public static void CleanTagList(ref string[] tags)
        {
            #if UNITY_EDITOR
            var susTags = tags;
            
            var oldLength = tags.Length;
            //remove duplicates
            tags = tags.Distinct().ToArray();
            if (tags.Length != oldLength)
            {
                Debug.LogWarning("Tag list had duplicates in it! This will be fixed in this case, but be careful. Bad tag list: ");
                foreach (var tag in susTags)
                {
                    Debug.LogWarning(tag);
                }
            }
            #endif
        }
    }

    [Serializable] private class TagCacheWrapper
    {
        [SerializeField] public List<string> TagList = null;
        
        public TagCacheWrapper(List<string> listToWrap)
        {
            TagList = listToWrap;
        }
    }
    
    /// <summary>
    /// Returns every gameobject containing a specific tag.
    /// If none are found, returns an empty list.
    /// </summary>
    /// <param name="tag">Tag the objects must all contain.</param>
    public static List<GameObject> FindGameObjectsWithTag(string tag)
    {
        if (objectStore.TryGetValue(tag, out var objects))
        {
            return objects;
        }

        return new List<GameObject>();
    }

    /// <summary>
    /// Returns all the tags (if any) attached to a game object.
    /// Will not be valid prior to Start().
    /// </summary>
    /// <param name="gameObject">The object with the tags to check.</param>
    public static List<string> GetAllGameObjectTags(GameObject gameObject)
    {
        if (tagStore.TryGetValue(gameObject, out var tags))
        {
            return tags.ToList();
        }
        
        return new List<string>();
    }
    
    /// <summary>
    /// Returns whether a gameobject has the specified tag.
    /// </summary>
    /// <param name="obj">The object to check tags on.</param>
    /// <param name="tag">The tag to compare against.</param>
    public static bool CompareTag(GameObject obj, string tag)
    {
        if (tagStore.TryGetValue(obj, out var tags))
        {
            return tags.Contains(tag);
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Returns whether a gameobject has at least one of the specified tags.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="anyTags"></param>
    public static bool CompareAnyTags(GameObject obj, IEnumerable<string> anyTags)
    {
        if (tagStore.TryGetValue(obj, out var tags))
        {
            return tags.Intersect(anyTags).ToList().Count > 0;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Returns whether a gameobject has ALL of the specified tags.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="anyTags"></param>
    public static bool CompareAllTags(GameObject obj, IEnumerable<string> anyTags)
    {
        if (tagStore.TryGetValue(obj, out var tags))
        {
            var tagCandidates = anyTags as string[] ?? anyTags.ToArray();
            return tags.Intersect(tagCandidates).Count() == tagCandidates.Count();
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Returns the first parent in the hierarchy that has the given tag.
    /// </summary>
    /// <param name="child">The object to consider as the bottom of the hierarchy.</param>
    /// <param name="tag">The tag to search for in the parents.</param>
    public static GameObject GetFirstParentWithTag(GameObject child, string tag)
    {
        var curTrans = child.transform.parent;
        while (curTrans)
        {
            if (CompareTag(curTrans.gameObject, tag))
            {
                return curTrans.gameObject;
            }
            curTrans = curTrans.parent;
        }

        return null;    }

    /// <summary>
    /// Returns the first parent in the hierarchy that has all the given tags.
    /// If none is found, returns null.
    /// </summary>
    /// <param name="child">The object to consider as the bottom of the hierarchy.</param>
    /// <param name="allTags">The tags to search for in the parents. The searched-for object must contain all of these to be counted.</param>
    public static GameObject GetFirstParentWithAllTags(GameObject child, IEnumerable<string> allTags)
    {
        var curTrans = child.transform.parent;
        while (curTrans)
        {
            if (CompareAllTags(curTrans.gameObject, allTags))
            {
                return curTrans.gameObject;
            }
            curTrans = curTrans.parent;
        }

        return null;
    }

    /// <summary>
    /// Returns the first parent in the hierarchy that has any of the given tags.
    /// If none is found, returns null.
    /// </summary>
    /// <param name="child">The object to consider as the bottom of the hierarchy.</param>
    /// <param name="anyTags">The tag to search for in the parents. The searched-for object must contain at least one of these to be counted.</param>
    public static GameObject GetFirstParentWithAnyTags(GameObject child, IEnumerable<string> anyTags)
    {
        var curTrans = child.transform.parent;
        while (curTrans)
        {
            if (CompareAnyTags(curTrans.gameObject, anyTags))
            {
                return curTrans.gameObject;
            }
            curTrans = curTrans.parent;
        }

        return null;
    }
    
    /// <summary>
    /// Returns all parents of the given object in the hierarchy that have the given tag.
    /// The list will be in order of lowest->highest in the hierarchy.
    /// If none are found, returns an empty list.
    /// </summary>
    /// <param name="child">The object to consider as the bottom of the hierarchy. It will not be included in the results.</param>
    /// <param name="tag">The tag to search for in the parents.</param>
    public static List<GameObject> GetAllParentsWithTag(GameObject child, string tag)
    {
        var curTrans = child.transform.parent;
        var objList = new List<GameObject>();
        while (curTrans)
        {
            if (CompareTag(curTrans.gameObject, tag))
            {
                objList.Add(curTrans.gameObject);
            }
            curTrans = curTrans.parent;
        }

        return objList;
    }

    /// <summary>
    /// Returns all parents of the given object in the hierarchy that have all of the given tags.
    /// The list will be in order of lowest->highest in the hierarchy.
    /// If none are found, returns an empty list.
    /// </summary>
    /// <param name="child">The object to consider as the bottom of the hierarchy.</param>
    /// <param name="allTags">The tags to search for in the parents. The searched-for objects must contain all of these to be counted.</param>
    public static List<GameObject> GetAllParentsWithAllTags(GameObject child, IEnumerable<string> allTags)
    {
        var curTrans = child.transform.parent;
        var objList = new List<GameObject>();
        while (curTrans)
        {
            if (CompareAllTags(curTrans.gameObject, allTags))
            {
                objList.Add(curTrans.gameObject);
            }
            curTrans = curTrans.parent;
        }

        return objList;
    }

    /// <summary>
    /// Returns all parents of the given object in the hierarchy that have at least one of the given tags.
    /// The list will be in order of lowest->highest in the hierarchy.
    /// If none are found, returns an empty list.
    /// </summary>
    /// <param name="child">The object to consider as the bottom of the hierarchy.</param>
    /// <param name="anyTags">The tag to search for in the parents. The searched-for object must contain at least one of these to be counted.</param>
    public static List<GameObject> GetAllParentsWithAnyTags(GameObject child, IEnumerable<string> anyTags)
    {
        var curTrans = child.transform.parent;
        var objList = new List<GameObject>();
        while (curTrans)
        {
            if (CompareAnyTags(curTrans.gameObject, anyTags))
            {
                objList.Add(curTrans.gameObject);
            }
            curTrans = curTrans.parent;
        }

        return objList;
    }
    
    //REGISTRATION AND TAG ADJUSTMENT - YOU SHOULD PROBABLY NOT BE CALLING THESE

    /// <summary>
    /// Registers an object and its tags, or updates its registry if one already exists.
    /// If none is found, returns null.
    /// </summary>
    /// <param name="obj">The object to update.</param>
    /// <param name="newTags">The additional tags that should be added.</param>
    public static void RegisterGameObjectTags(GameObject obj, string[] newTags)
    {
        if (tagStore.TryGetValue(obj, out var tags))
        {
            //append new tags to old tags
            var newArrayBegin = tags.Length;
            Array.Resize(ref tags,  tags.Length + newTags.Length);
            Array.Copy(newTags, 0, tags, newArrayBegin, newTags.Length);
            
            TagValidator.CleanTagList(ref tags);
            
            //update entry
            tagStore[obj] = tags;
        }
        else
        {
            TagValidator.CleanTagList(ref newTags);
            tagStore.Add(obj, newTags);
        }
        
        //update object store too
        foreach (var tag in newTags)
        {
            if (objectStore.TryGetValue(tag, out var objects))
            {
                objects.Add(obj);
            }
            else
            {
                objectStore[tag] = new List<GameObject> {obj};
            }
        }
    }

    /// <summary>
    /// Remove tags from a gameobject.
    /// </summary>
    /// <param name="obj">The object to untag.</param>
    /// <param name="tags">The tags to remove.</param>
    public static void UnregisterGameObjectTags(GameObject obj, string[] tags)
    {
        //First update object entry
        if (tagStore.TryGetValue(obj, out var convertedTags))
        {
            var nTags = convertedTags.Except(tags).ToArray();
            if (nTags.Length == 0)
            {
                //remove objects with no tags from the system, making lookups faster
                tagStore.Remove(obj);
            }
            else
            {
                tagStore[obj] = nTags;
            }
        }
        else
        {
            Debug.LogWarning("Attempted to remove tags from object which didn't have any");
        }
        
        //Also update corresponding tag entries
        foreach (var tag in tags)
        {
            if (objectStore.TryGetValue(tag, out var objects))
            {
                objects.Remove(obj);
            }
        }
    }

    /// <summary>
    /// Registers a tag to an object, or updates its registry if it already exists.
    /// </summary>
    /// <param name="obj">The object to update.</param>
    /// <param name="tag">The tag to add.</param>
    public static void RegisterGameObjectTag(GameObject obj, string tag)
    {
        RegisterGameObjectTags(obj, new []{tag});
    }

    /// <summary>
    /// Remove a tag from a gameobject.
    /// </summary>
    /// <param name="obj">The object to remove the tag from.</param>
    /// <param name="tag">The tag to remove.</param>
    public static void UnregisterGameObjectTag(GameObject obj, string tag)
    {
        UnregisterGameObjectTags(obj, new[] {tag});
    }

    /// <summary>
    /// Remove all tags from a gameobject.
    /// </summary>
    /// <param name="obj">The object to de-tag.</param>
    public static void UnregisterGameObjectTags(GameObject obj)
    {
        if (tagStore.TryGetValue(obj, out var tags))
        {
            tagStore.Remove(obj);
            foreach (var tag in tags)
            {
                if (objectStore.TryGetValue(tag, out var objects))
                {
                    objects.Remove(obj);
                }
                else
                {
                    Debug.LogWarning("Tag was present in tag store but not object store!"
                                     + " Something went really wrong...");
                }
            }
        }
    }

    //serialization

    
    
    /// <summary>
    /// Commits the current tag cache to disk.
    /// If the tag cache is missing tags present on disk, they will be combined.
    /// To remove tags, use <see cref="DestroyTag"/>.
    /// </summary>
    private static void SerializeTagCache()
    {
        var permTags = new TagCacheWrapper(new List<string>());
        try
        {
            var listReader = new StreamReader(tagStorePath, Encoding.ASCII);
            var oldTagJson = listReader.ReadToEnd();
#if UNITY_EDITOR
            EditorJsonUtility.FromJsonOverwrite(oldTagJson, permTags);
#endif
            listReader.Close();
        }
        catch (FileNotFoundException)
        {
            //no tag cache, just do nothing and let it be created
            Debug.Log("No tag cache found, creating one...");
        }
       
        permTags.TagList.AddRange(_tagCache.TagList);
        permTags.TagList = permTags.TagList.Distinct().ToList();
        permTags.TagList.Sort();
        
        WriteTagCache(permTags);
    }

    /// <summary>
    /// Removes a tag from existence, completely.
    /// It will not appear in dropdowns any longer.
    /// </summary>
    /// <param name="tag">The tag to destroy.</param>
    public static void DestroyTag(string tag)
    {
        try
        {
            var listReader = new StreamReader(tagStorePath, Encoding.ASCII);
            var oldTagJson = listReader.ReadToEnd();
            var tempList = new TagCacheWrapper(new List<string>());
#if UNITY_EDITOR
            EditorJsonUtility.FromJsonOverwrite(oldTagJson, tempList);
#endif
            listReader.Close();

            tempList.TagList.Remove(tag);
            WriteTagCache(tempList);
            
            LoadTagCache();
        }
        catch (FileNotFoundException)
        {
            //nothing to delete i guess :P
        }
    }

    private static void WriteTagCache(TagCacheWrapper cache)
    {
#if UNITY_EDITOR
        try
        {
            var listWriter = new StreamWriter(tagStorePath, false);

            var newTagJson = EditorJsonUtility.ToJson(cache);

            listWriter.Write(newTagJson);
            listWriter.Close();
        }
        catch (IOException e)
        {
            Debug.LogError("Failed to write tag cache!!!: " + e.Message);
        }
#endif
    }

    /// <summary>
    /// Adds a new tag to the permanent tag registry.
    /// It will appear in tag selection dropdowns.
    ///
    /// Also reloads the tag cache.
    /// </summary>
    /// <param name="tags">The tags to add.</param>
    public static void AddTags(List<string> tags)
    {
        _tagCache.TagList.AddRange(tags);
        SerializeTagCache();
        LoadTagCache();
    }
    
    /// <summary>
    /// Loads all the current tags from the tag cache.
    /// Deletes the current cache, as well.
    /// </summary>
    private static void LoadTagCache()
    {
        _tagCache.TagList.Clear();

        try
        {
            var listReader = new StreamReader(tagStorePath, Encoding.ASCII);
            var oldTagJson = listReader.ReadToEnd();
            var tempList = new TagCacheWrapper(new List<string>());
#if UNITY_EDITOR
            EditorJsonUtility.FromJsonOverwrite(oldTagJson, tempList);
#endif
            _tagCache.TagList = tempList.TagList;
            listReader.Close();
        }
        catch (FileNotFoundException)
        {
            //just haven't made a tag cache yet. \shrug
        }
    }

    public static ReadOnlyCollection<string> AllTags => _tagCache.TagList.AsReadOnly();

    //these lookup lists are used to make tag lookup faster
    private static readonly TagCacheWrapper _tagCache = new TagCacheWrapper(new List<string>());
    private static readonly Dictionary<GameObject, string[]> tagStore = new Dictionary<GameObject, string[]>();
    private static readonly Dictionary<string, List<GameObject>> objectStore = new Dictionary<string, List<GameObject>>();

    private static readonly string tagStorePath = Application.persistentDataPath + "/tagStore.json";
}
