using Moq;
using produtos.core;
using produtos.services;
using FluentAssertions;

namespace produtos.Tests;

public class ProdutoServiceTests
{
    private readonly Mock<IProdutoRepository> produtoRepository;
    private readonly ProdutoService produtoService;

    public ProdutoServiceTests()
    {
        produtoRepository = new Mock<IProdutoRepository>();
        produtoService = new ProdutoService(produtoRepository.Object);
    }

    [Fact(DisplayName = "Obter Produto por ID")]
    public void ObterProdutoPorID_DeveRetornarProdutoCorreto()
    {
        // Arrange
        var produto = new Produto(1, "Cabo USB", 10.0);
        produtoRepository.Setup(x => x.GetById(1)).Returns(produto);

        // Act
        var result = produtoService.GetProduto(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(produto);
    }

    [Fact(DisplayName = "Salvar Produto Válido")]
    public void SalvarProduto_Valido_DeveSalvarProdutoCorreto()
    {
        // Arrange
        var produto = new Produto(1, "Carregador", 50.0);

        // Act
        produtoService.SalvarProduto(produto);

        // Assert
        produtoRepository.Verify(x => x.Save(produto), Times.Once);
    }

    [Fact(DisplayName = "Salvar Produto Inválido")]
    public void SalvarProduto_Invalido_DeveLancarExcecao()
    {
        // Arrange
        var produto = new Produto(1, "", 50.0);

        // Act & Assert
        FluentActions.Invoking(() => produtoService.SalvarProduto(produto))
            .Should().Throw<ArgumentException>();
        produtoRepository.Verify(x => x.Save(It.IsAny<Produto>()), Times.Never);
    }

    [Fact(DisplayName = "Atualizar Produto Existente")]
    public void AtualizarProduto_Existente_DeveAtualizarProdutoCorreto()
    {
        // Arrange
        var produto = new Produto(1, "Carregador", 50.0);
        produtoRepository.Setup(x => x.GetById(produto.Id)).Returns(produto);

        // Act
        produtoService.AtualizarProduto(produto);

        // Assert
        produtoRepository.Verify(x => x.Update(produto), Times.Once);
    }

    [Fact(DisplayName = "Atualizar Produto Inexistente")]
    public void AtualizarProduto_Inexistente_DeveLancarExcecao()
    {
        // Arrange
        var produto = new Produto(1, "Carregador", 50.0);

        // Act
        Action atualizarProduto = () => produtoService.AtualizarProduto(produto);

        // Assert
        atualizarProduto.Should().Throw<InvalidOperationException>()
            .WithMessage($"Não é possível atualizar o produto com ID {produto.Id} porque não foi encontrado.");

        produtoRepository.Verify(x => x.Update(It.IsAny<Produto>()), Times.Never);
    }

    [Fact(DisplayName = "Excluir Produto Existente")]
    public void ExcluirProduto_Existente_DeveExcluirProdutoCorreto()
    {
        // Arrange
        var produto = new Produto(1, "Carregador", 50.0);
        produtoRepository.Setup(x => x.GetById(1)).Returns(produto);

        // Act
        produtoService.ExcluirProduto(1);

        // Assert
        produtoRepository.Verify(x => x.Delete(1), Times.Once);
    }

    [Fact(DisplayName = "Excluir Produto Inexistente")]
    public void ExcluirProduto_Inexistente_DeveLancarExcecao()
    {
        // Arrange
        int produtoId = 1;

        // Act
        Action excluirProduto = () => produtoService.ExcluirProduto(produtoId);

        // Assert
        excluirProduto.Should().Throw<InvalidOperationException>()
            .WithMessage($"Não é possível excluir o produto com ID {produtoId} porque não foi encontrado.");

        produtoRepository.Verify(x => x.Delete(It.IsAny<int>()), Times.Never);
    }

    [Fact(DisplayName = "Obter Todos os Produtos")]
    public void ObterTodosProdutos_DeveRetornarTodosOsProdutos()
    {
        // Arrange
        var listaProdutos = new List<Produto>
        {
            new Produto(1, "Produto1", 10.0),
            new Produto(2, "Produto2", 20.0)
        };
        produtoRepository.Setup(x => x.GetAll()).Returns(listaProdutos);

        // Act
        var result = produtoService.ObterTodosProdutos();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(listaProdutos);
    }
}