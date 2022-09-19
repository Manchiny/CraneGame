using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameInfo : ScriptableObject
{
    [SerializeField] private List<GameInfoPage> _pages;

    public int PagesCount => _pages.Count;

    public bool TryGetPage(int pageNumber, out GameInfoPage page)
    {
        if (pageNumber >= 0 && pageNumber < _pages.Count)
        {
            page = _pages[pageNumber];
            return true;
        }
        else
        {
            page = null;
            return false;
        }
    }
}
