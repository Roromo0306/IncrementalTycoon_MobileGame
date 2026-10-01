public interface IIncomeModifier
{
    Money Apply(Money currentValue, IncomeContext context);
}