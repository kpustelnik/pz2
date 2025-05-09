namespace MvcLab9.ViewModels;
using MvcLab9.Models;

public class DataViewModel {
    public DataViewModel(List<Data> d) {
        Data = d;
    }
    public List<Data> Data { get; set; }
}
