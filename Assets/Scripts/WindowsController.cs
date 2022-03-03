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
    /// Показывает определенный экран и выполняет над ним действие после иницализации. Если открыт другой экран
    /// то закрывает текущий и открывает новый. При возврате открывает предыдущий. 
    /// </summary>
    /// <typeparam name="T"> Класс экрана для отображения </typeparam>
    /// <param name="closeAllOther"> Если истина то делает экран первым в истории экранов, предыдущие забываются. </param>
    /// <param name="action"> Действие выполняемое после инициализации </param>
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
