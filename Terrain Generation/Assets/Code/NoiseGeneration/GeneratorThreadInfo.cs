using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GeneratorThreadInfo<T>
{
	public readonly Action<T> callback;
	public readonly T parameter;

	public GeneratorThreadInfo(Action<T> callback, T parameter)
	{
		this.callback = callback;
		this.parameter = parameter;
	}
}