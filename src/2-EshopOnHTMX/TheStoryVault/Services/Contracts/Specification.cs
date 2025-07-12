using System.Linq.Expressions;

namespace TheStoryVault.Services.Contracts;

public abstract class Specification<T>
    where T : class
{
    public abstract Expression<Func<T, bool>> GetCriteria();

    public Specification<T> And(Specification<T> other)
    {
        return new AndSpecification<T>(this, other);
    }

    public Specification<T> Or(Specification<T> other)
    {
        return new OrSpecification<T>(this, other);
    }

    public Specification<T> Not()
    {
        return new NotSpecification<T>(this);
    }
}

internal class AndSpecification<T> : Specification<T>
    where T : class
{
    private readonly Specification<T> left;
    private readonly Specification<T> right;

    public AndSpecification(Specification<T> left, Specification<T> right)
    {
        this.left = left;
        this.right = right;
    }

    public override Expression<Func<T, bool>> GetCriteria()
    {
        Expression<Func<T, bool>> leftEpr = this.left.GetCriteria();
        Expression<Func<T, bool>> rirth = this.right.GetCriteria();
        BinaryExpression exp = Expression<Func<T, bool>>.AndAlso(leftEpr, rirth);
        return Expression.Lambda<Func<T, bool>>(exp, leftEpr.Parameters);
    }
}

internal class OrSpecification<T> : Specification<T>
    where T : class
{
    private readonly Specification<T> left;
    private readonly Specification<T> right;

    public OrSpecification(Specification<T> left, Specification<T> right)
    {
        this.left = left;
        this.right = right;
    }

    public override Expression<Func<T, bool>> GetCriteria()
    {
        Expression<Func<T, bool>> leftEpr = this.left.GetCriteria();
        Expression<Func<T, bool>> rirth = this.right.GetCriteria();
        BinaryExpression exp = Expression<Func<T, bool>>.OrElse(leftEpr, rirth);
        return Expression.Lambda<Func<T, bool>>(exp, leftEpr.Parameters);
    }
}

internal class NotSpecification<T> : Specification<T>
    where T : class
{
    private readonly Specification<T> left;

    public NotSpecification(Specification<T> left)
    {
        this.left = left;
    }

    public override Expression<Func<T, bool>> GetCriteria()
    {
        Expression<Func<T, bool>> leftEpr = this.left.GetCriteria();
        UnaryExpression exp = Expression<Func<T, bool>>.Not(leftEpr);
        return Expression.Lambda<Func<T, bool>>(exp, leftEpr.Parameters);
    }
}