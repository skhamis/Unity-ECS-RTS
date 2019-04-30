using Unity.Entities;

//For tagging purposes only, tags the highlight
[System.Serializable]
public struct Highlight : IComponentData
{
}

public class HighlightComponent : ComponentDataProxy<Highlight> { }
