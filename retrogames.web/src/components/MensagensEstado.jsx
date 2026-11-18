export function Carregando({ texto = 'Carregando...' }) {
  return (
    <p role="status" className="text-secondary">
      {texto}
    </p>
  )
}

export function MensagemErro({
  mensagem,
  aoTentarNovamente
}) {
  return (
    <div className="alert alert-danger" role="alert">
      <p className="mb-2">{mensagem}</p>

      {aoTentarNovamente && (
        <button
          type="button"
          className="btn btn-outline-danger"
          onClick={aoTentarNovamente}
        >
          Tentar novamente
        </button>
      )}
    </div>
  )
}

export function EstadoVazio({
  titulo = 'Nenhum resultado encontrado.',
  children
}) {
  return (
    <div className="alert alert-light border" role="status">
      <h2 className="h5">{titulo}</h2>
      {children && <p className="mb-0">{children}</p>}
    </div>
  )
}