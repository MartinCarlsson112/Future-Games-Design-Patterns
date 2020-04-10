using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class InjectAttribute : Attribute
{
}

/// <summary>
///  Barebones implementation of Dependency Injection for Unity
///  Dependencies are injected with the first object of the type in the scene hierarchy, from GameObject.FindObjectOfType(type)
/// </summary>
public class DependencyInjector : MonoBehaviour
{
    private void Awake()
    {
        Inject();
    }

    private bool IsMemberInjectable(MemberInfo member)
    {
        return member.GetCustomAttributes(true).Where(attribute => attribute is InjectAttribute).Count() > 0;
    }

    private void GetAllInjectableObjects(in GameObject[] allGameObjects, out List<MonoBehaviour> injectables)
    {
        injectables = new List<MonoBehaviour>();
        foreach(var gameObject in allGameObjects)
        {
            MonoBehaviour[] components = gameObject.GetComponents<MonoBehaviour>();
            foreach (var component in components)
            {
                Type compType = component.GetType();
                bool compHasInjectableProperties = compType.GetProperties().Where(IsMemberInjectable).Any();
                if(compHasInjectableProperties)
                {
                    injectables.Add(component);
                }
                else
                {
                    bool compHasInjectableFields = compType.GetFields().Where(IsMemberInjectable).Any();
                    if(compHasInjectableFields)
                    {
                        injectables.Add(component);
                    }
                }
            }
        }
    }

    private void InjectDependencies(MonoBehaviour injectable)
    {
        Type type = injectable.GetType();

        PropertyInfo[] properties = type.GetProperties().Where(IsMemberInjectable).ToArray();
        FieldInfo[] fields = type.GetFields().Where(IsMemberInjectable).ToArray();

        foreach(var property in properties)
        {
            Type propertyType = property.PropertyType;
            
            if(property.SetMethod.IsPrivate)
            {
                Debug.LogError("Injection Attribute used on a private member field: " + property.Name + "in component" + type.Name);
                continue;
            }
            if (propertyType.IsArray)
            {
                Type elementType = propertyType.GetElementType();
                var allObjects = GameObject.FindObjectsOfType(elementType);
                var arrayToCopy = Array.CreateInstance(elementType, allObjects.Length);
                Array.Copy(allObjects, arrayToCopy, allObjects.Length);
                property.SetValue(injectable, arrayToCopy);
            }
            else
            {
                property.SetValue(injectable, GameObject.FindObjectOfType(propertyType));
            }
            
        }
        foreach (var field in fields)
        {
            Type fieldType = field.FieldType;
            if(field.IsPrivate)
            {
                Debug.LogError("Injection Attribute used on a private member field: " + field.Name + "in component" + type.Name);
                continue;
            }
            if (fieldType.IsArray)
            {
                Type elementType = fieldType.GetElementType();
                var allObjects = GameObject.FindObjectsOfType(elementType);
                var arrayToCopy = Array.CreateInstance(elementType, allObjects.Length);
                Array.Copy(allObjects, arrayToCopy, allObjects.Length);
                field.SetValue(injectable, arrayToCopy);
            }
            else
            {
                field.SetValue(injectable, GameObject.FindObjectOfType(fieldType));
            }
            
        }
    }

    private void Inject()
    {
        GameObject[] allGameObjects = GameObject.FindObjectsOfType<GameObject>();
        GetAllInjectableObjects(allGameObjects, out List<MonoBehaviour> injectables);
        foreach(var injectable in injectables)
        {
            InjectDependencies(injectable);
        }
    }
}
