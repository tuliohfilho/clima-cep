import axios from 'axios';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5137/api';

const cepService = {
  /**
   * Busca informações de localização por CEP
   * @param {string} cep - CEP a ser buscado (sem formatação)
   * @returns {Promise} Dados de localização
   */
  searchCep: async (cep) => {
    try {
      const cleanCep = cep.replace(/\D/g, '');
      const response = await axios.get(`${API_BASE_URL}/cep/${cleanCep}`);
      return response.data;
    } catch (error) {
      throw new Error(error.response?.data?.message || 'Erro ao buscar CEP');
    }
  },

  /**
   * Busca informações de clima
   * @param {number} latitude - Latitude da localização
   * @param {number} longitude - Longitude da localização
   * @returns {Promise} Dados de clima
   */
  getWeather: async (latitude, longitude) => {
    try {
      const response = await axios.get(`${API_BASE_URL}/weather`, {
        params: {
          latitude,
          longitude
        }
      });
      return response.data;
    } catch (error) {
      throw new Error(error.response?.data?.message || 'Erro ao buscar clima');
    }
  }
};

export default cepService;
