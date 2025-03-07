using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace QuizeManagement.Models
{
    public class QuizModel
    {
        [Required(ErrorMessage = "User name is required")]
        public int UserID { get; set; }

        public int QuizID { get; set; }

        [Required(ErrorMessage = "Quiz Name is Required")]
        public string QuizName { get; set; }

        [Required(ErrorMessage = "Total Question is Required")]
        [Range(1,100 ,ErrorMessage = "Total Questions must greater then 0.")]
        public int TotalQuestions { get; set; }

        [Required(ErrorMessage = "Quiz Date is required.")]
        [FutureDate(ErrorMessage = "Date must be today or in the future.")]
        public DateTime QuizDate { get; set; }
         
        public DateTime Created { get; set; }
        public DateTime Modified {  get; set; }
        public int QuestionID { get; internal set; }
        public string? QuestionLevel { get; internal set; }
    }

    public class QuizDropdownModel
    {
        public int QuizID { get; set; }
        public string QuizName { get; set; }
    }

    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime date)
            {
                if (date < DateTime.Today)
                {
                    return new ValidationResult(ErrorMessage ?? "Date must be today or in the future.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
