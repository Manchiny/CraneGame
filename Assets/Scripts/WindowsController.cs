using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class WindowsController : MonoBehaviour
{
    [SerializeField] private RectTransform _windowHolder;
    public ReactiveProperty<AbstractWindow> CurrentWindow { get; } = new ReactiveProperty<AbstractWindow>(null);

    /// <summary>
    /// ���������� ������������ ����� � ��������� ��� ��� �������� ����� ������������. ���� ������ ������ �����
    /// �� ��������� ������� � ��������� �����. ��� �������� ��������� ����������. 
    /// </summary>
    /// <typeparam name="T"> ����� ������ ��� ����������� </typeparam>
    /// <param name="closeAllOther"> ���� ������ �� ������ ����� ������ � ������� �������, ���������� ����������. </param>
    /// <param name="action"> �������� ����������� ����� ������������� </param>
    public T ScreenChange<T>(bool closeAllOther = false, Action<T> action = null) where T : AbstractWindow
    {
        if (closeAllOther)
        {
            if (CurrentWindow.Value != null)
                CloseCurrentWindow();
        }

        return OpenScreen(action);
    }

    private T OpenScreen<T>(Action<T> action, Dictionary<string, object> addLogParams = null) where T : AbstractWindow
    {
        var window = GetWindow<T>();
        if (window != null)
        {
            window = Instantiate(window, _windowHolder);
            action.Invoke(window);
        }

        return window;
    }

    private void CloseCurrentWindow()
    {
        CurrentWindow?.Value.Close();
    }

    private T GetWindow<T>() where T : AbstractWindow
    {
        Type type = typeof(T);
        T window = null;
        if (WindowsHolder.Windows.ContainsKey(type))
        {
            window = Resources.Load<T>(WindowsHolder.Windows[type]);
        }

        return window;
    }
}
