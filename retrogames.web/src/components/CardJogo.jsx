import { Link } from 'react-router-dom'
import { formatarPreco } from '../lib/formato'

function CardJogo({ jogo }) {
  const esgotado = jogo.estoque <= 0

  return (
    <div className="card h-100 shadow-sm">
      <div className="card-body d-flex flex-column">
        <div className="d-flex justify-content-between align-items-start mb-2">
          <span className="badge text-bg-secondary">
            {jogo.nomePlataforma}
          </span>

          {esgotado && (
            <span className="badge text-bg-danger">
              Esgotado
            </span>
          )}
        </div>

        <h2 className="card-title h5">{jogo.nome}</h2>

        <p className="text-secondary mb-1">
          Ano: {jogo.anoLancamento}
        </p>

        <p className="text-secondary mb-1">
          Condição: {jogo.descricaoCondicao}
        </p>

        <p className="mb-2">
          Gêneros: {jogo.generos?.join(', ')}
        </p>

        <p className="fw-bold fs-5 mt-auto mb-3">
          {formatarPreco(jogo.preco)}
        </p>

        <Link
          to={`/jogos/${jogo.id}`}
          className="btn btn-primary"
        >
          Ver detalhes
        </Link>
      </div>
    </div>
  )
}

export default CardJogo