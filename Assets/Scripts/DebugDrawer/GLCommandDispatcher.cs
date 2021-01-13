using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Dispatcher class for GLCommand
/// </summary>
public class GLCommandDispatcher : MonoBehaviour
{
    public static GLCommandDispatcher Instance;

    private List<DrawCommand> commandQueue;

    private void Start()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        commandQueue = new List<DrawCommand>();
    }

    public void OnRenderObject()
    {
        if (commandQueue.Count == 0) return;

        if (Camera.current.CompareTag("MainCamera"))
        {
            GL.PushMatrix();
            var cam = Camera.main;

            GL.LoadProjectionMatrix(cam.projectionMatrix);
            GL.modelview = cam.worldToCameraMatrix;

            foreach (var command in commandQueue)
            {
                command.Execute();
            }

            GL.PopMatrix();
        }

        commandQueue.RemoveAll(x => x.IsTimeOut());
    }



    public void Publish(DrawCommand command)
    {
        commandQueue.Add(command);
    }
}