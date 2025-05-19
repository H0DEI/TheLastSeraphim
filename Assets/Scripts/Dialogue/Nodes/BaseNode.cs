using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using XNode;

public class BaseNode : Node {

	// Use this for initialization

	[Input] public int entry;
	[Output] public int exit;

    public List<TwoStrings> animations;

    protected override void Init() {
		base.Init();
	}

	public virtual System.Type ReturnType()
    {
		return typeof(BaseNode);
    }

	public virtual void Execute()
    {

        Debug.Log("Executing node");
    }

	public virtual BaseNode NextNode(string _exit)
    {
		foreach (NodePort p in this.Ports)
        {
			if (p.fieldName == _exit)
            {
				return p.Connection.node as BaseNode;
            }
        }

		return null;
	}
}