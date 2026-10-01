public interface IIncomeCalculator
{
    Money Calculate(Money baseIncome,IncomeContext context);
}