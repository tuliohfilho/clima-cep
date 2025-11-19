import axios from 'axios';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5137/api';

const historyService = {
  /**
   * Obtém histórico de buscas do servidor
   * @returns {Promise} Array de buscas do histórico
   */
  getHistory: async () => {
    try {
      const response = await axios.get(`${API_BASE_URL}/history`);
      return response.data;
    } catch (error) {
      throw new Error(error.response?.data?.message || 'Erro ao obter histórico');
    }
  },

  /**
   * Adiciona uma busca ao histórico
   * @param {object} searchData - Dados da busca
   * @returns {Promise} Resposta do servidor
   */
  addToHistory: async (searchData) => {
    try {
      const response = await axios.post(`${API_BASE_URL}/history`, searchData);
      return response.data;
    } catch (error) {
      throw new Error(error.response?.data?.message || 'Erro ao adicionar ao histórico');
    }
  },

  /**
   * Remove uma entrada do histórico
   * @param {string} id - ID do histórico a remover
   * @returns {Promise} Resposta do servidor
   */
  removeFromHistory: async (id) => {
    try {
      const response = await axios.delete(`${API_BASE_URL}/history/${id}`);
      return response.data;
    } catch (error) {
      throw new Error(error.response?.data?.message || 'Erro ao remover do histórico');
    }
  },

  /**
   * Limpa todo o histórico
   * @returns {Promise} Resposta do servidor
   */
  clearHistory: async () => {
    try {
      const response = await axios.delete(`${API_BASE_URL}/history`);
      return response.data;
    } catch (error) {
      throw new Error(error.response?.data?.message || 'Erro ao limpar histórico');
    }
  },

  /**
   * Obtém detalhes de uma busca específica
   * @param {string} id - ID da busca
   * @returns {Promise} Dados da busca
   */
  getHistoryDetail: async (id) => {
    try {
      const response = await axios.get(`${API_BASE_URL}/history/${id}`);
      return response.data;
    } catch (error) {
      throw new Error(error.response?.data?.message || 'Erro ao obter detalhes do histórico');
    }
  }
};

export default historyService;
