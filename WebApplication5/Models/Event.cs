using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication5.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Display(Name = "Название")]
        [Required(ErrorMessage = "Введите название мероприятия.")]
        [StringLength(200, ErrorMessage = "Название не должно превышать 200 символов.")]
        public string Title { get; set; }

        [Display(Name = "Описание")]
        [StringLength(4000, ErrorMessage = "Описание не должно превышать 4000 символов.")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(Name = "Дата и время")]
        [Range(typeof(DateTime), "2000-01-01", "2099-12-31", ErrorMessage = "Укажите допустимые дату и время начала.")]
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }

        [Display(Name = "Цена")]
        [Range(typeof(decimal), "0", "1000000", ErrorMessage = "Цена должна быть от 0 до 1 000 000 рублей.")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Display(Name = "Количество мест")]
        [Range(1, 100000, ErrorMessage = "Укажите количество мест от 1 до 100 000.")]
        public int Seats { get; set; }

        public bool IsPublished { get; set; }

        [Display(Name = "Категория")]
        [Range(1, int.MaxValue, ErrorMessage = "Выберите категорию мероприятия.")]
        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }
    }
}