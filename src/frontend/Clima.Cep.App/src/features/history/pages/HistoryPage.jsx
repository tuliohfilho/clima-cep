import React from 'react';
import HistoryList from '../components/HistoryList';
import HistoryDetail from '../components/HistoryDetail';
import useHistory from '../hooks/useHistory';
import './HistoryPage.css';

/**
 * Página de histórico de buscas
 */
const HistoryPage = () => {
  const {
    historyData,
    loading,
    selectedItem,
    setSelectedItem,
    removeItem,
    clearAll,
    getDetail
  } = useHistory();

  const handleSelectItem = (item) => {
    setSelectedItem(item);
  };

  const handleRemoveItem = (id) => {
    if (window.confirm('Tem certeza que deseja remover este item?')) {
      removeItem(id);
    }
  };

  const handleClearAll = () => {
    if (window.confirm('Tem certeza que deseja limpar todo o histórico? Esta ação não pode ser desfeita.')) {
      clearAll();
    }
  };

  return (
    <div className="history-page">
      <div className="page-header">
        <h1>Histórico de Buscas</h1>
        <p>Veja todas as suas buscas anteriores de CEP</p>
      </div>

      {!loading && (
        <div className="history-container">
          <div className="history-list-section">
            {historyData.length === 0 ? (
              <div className="empty-state">
                <p>Nenhuma busca realizada ainda</p>
              </div>
            ) : (
              <>
                {historyData.length > 0 && (
                  <button className="btn btn-danger" onClick={handleClearAll}>
                    Limpar Todo Histórico
                  </button>
                )}
                <HistoryList
                  items={historyData}
                  onSelectItem={handleSelectItem}
                  onRemoveItem={handleRemoveItem}
                  selectedId={selectedItem?.id}
                />
              </>
            )}
          </div>

          <div className="history-detail-section">
            {selectedItem && (
              <HistoryDetail
                data={selectedItem}
                onClose={() => setSelectedItem(null)}
              />
            )}
          </div>
        </div>
      )}
    </div>
  );
};

export default HistoryPage;
