import React from 'react';
import './CepSearchForm.css';

/**
 * Componente de formulário para busca de CEP
 */
const CepSearchForm = ({ onSearch, loading }) => {
  const [cepInput, setCepInput] = React.useState('');

  const handleSubmit = (e) => {
    e.preventDefault();
    onSearch(cepInput);
  };

  const handleChange = (e) => {
    let value = e.target.value.replace(/\D/g, '');
    // Formatar CEP: XXXXX-XXX
    if (value.length <= 5) {
      setCepInput(value);
    } else {
      setCepInput(value.substring(0, 5) + '-' + value.substring(5, 8));
    }
  };

  return (
    <form className="cep-search-form" onSubmit={handleSubmit}>
      <div className="form-group">
        <label htmlFor="cep-input">CEP:</label>
        <input
          id="cep-input"
          type="text"
          placeholder="Digite um CEP (ex: 01310-100)"
          value={cepInput}
          onChange={handleChange}
          maxLength="9"
          disabled={loading}
          className="cep-input"
          required
        />
      </div>
      <button type="submit" disabled={loading} className="btn btn-primary">
        {loading ? 'Buscando...' : 'Buscar'}
      </button>
    </form>
  );
};

export default CepSearchForm;
