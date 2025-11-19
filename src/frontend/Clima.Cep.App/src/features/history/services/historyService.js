import axios from 'axios';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5137/api';
const STORAGE_KEY = 'cepSearchHistory';

const historyService = {
  /**
   * Obtém histórico de buscas (localStorage como fallback)
   * @returns {Promise} Array de buscas do histórico
   */
  getHistory: async () => {
    try {
      try {
        const response = await axios.get(`${API_BASE_URL}/history`);
        return response.data || [];
      } catch (apiError) {
        // Se a API falhar, usa localStorage
        const saved = localStorage.getItem(STORAGE_KEY);
        return saved ? JSON.parse(saved) : [];
      }
    } catch (error) {
      // Fallback final
      return [];
    }
  },

  /**
   * Adiciona uma busca ao histórico
   * @param {object} searchData - Dados da busca
   * @returns {Promise} Resposta do servidor
   */
  addToHistory: async (searchData) => {
    try {
      try {
        const response = await axios.post(`${API_BASE_URL}/history`, searchData);
        return response.data;
      } catch (apiError) {
        // Se a API falhar, salva localmente
        const history = JSON.parse(localStorage.getItem(STORAGE_KEY) || '[]');
        const newEntry = {
          id: new Date().getTime().toString(),
          ...searchData,
          timestamp: new Date().toISOString()
        };
        history.unshift(newEntry);
        localStorage.setItem(STORAGE_KEY, JSON.stringify(history.slice(0, 20)));
        return newEntry;
      }
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
      try {
        const response = await axios.delete(`${API_BASE_URL}/history/${id}`);
        return response.data;
      } catch (apiError) {
        // Se a API falhar, remove do localStorage
        const history = JSON.parse(localStorage.getItem(STORAGE_KEY) || '[]');
        const filtered = history.filter(item => item.id !== id);
        localStorage.setItem(STORAGE_KEY, JSON.stringify(filtered));
        return { success: true };
      }
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
      try {
        const response = await axios.delete(`${API_BASE_URL}/history`);
        return response.data;
      } catch (apiError) {
        // Se a API falhar, limpa localStorage
        localStorage.removeItem(STORAGE_KEY);
        return { success: true };
      }
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
      try {
        const response = await axios.get(`${API_BASE_URL}/history/${id}`);
        return response.data;
      } catch (apiError) {
        // Se a API falhar, busca do localStorage
        const history = JSON.parse(localStorage.getItem(STORAGE_KEY) || '[]');
        return history.find(item => item.id === id) || null;
      }
    } catch (error) {
      throw new Error(error.response?.data?.message || 'Erro ao obter detalhes do histórico');
    }
  }
};

export default historyService;
