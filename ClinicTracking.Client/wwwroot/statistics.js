// Chart.js utility functions for statistics dashboard

let chartInstances = {};

function destroyChart(chartId) {
    if (chartInstances[chartId]) {
        chartInstances[chartId].destroy();
        delete chartInstances[chartId];
    }
}

function showFallback(chartId) {
    const canvas = document.getElementById(chartId);
    const fallback = document.getElementById(chartId + 'Fallback');
    if (canvas) canvas.style.display = 'none';
    if (fallback) fallback.style.display = 'block';
}

// Returns the canvas element if valid, or null if chart should not be rendered
function getValidCanvas(chartId) {
    if (typeof Chart === 'undefined') {
        showFallback(chartId);
        return null;
    }
    // Check if canvas element exists
    const canvas = document.getElementById(chartId);
    if (!canvas) {
        console.warn(`Chart canvas element '${chartId}' not found`);
        showFallback(chartId);
        return null;
    }
    return canvas;
}

// Utility function to safely handle null/undefined data
function safeArray(data) {
    if (!Array.isArray(data)) return [];
    // Filter out null/undefined entries from the array
    return data.filter(item => item !== null && item !== undefined);
}

// Utility function to safely get a value with a default
function safeValue(value, defaultValue) {
    return (value === null || value === undefined) ? defaultValue : value;
}

// Utility function to safely get a string with a default
function safeString(value, defaultValue) {
    if (value === null || value === undefined || value === '') {
        return defaultValue || 'Unspecified';
    }
    return value;
}

window.renderWaitTimeChart = function(data) {
    const canvas = getValidCanvas('waitTimeChart');
    if (!canvas) return;
    
    destroyChart('waitTimeChart');
    
    try {
        const safeData = safeArray(data);
        const ctx = canvas.getContext('2d');
        chartInstances['waitTimeChart'] = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: safeData.map(d => safeString(d.range, 'Unknown')),
                datasets: [{
                    label: 'Number of Patients',
                    data: safeData.map(d => safeValue(d.count, 0)),
                    backgroundColor: 'rgba(54, 162, 235, 0.6)',
                    borderColor: 'rgba(54, 162, 235, 1)',
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            stepSize: 1
                        }
                    }
                },
                plugins: {
                    legend: {
                        display: false
                    }
                }
            }
        });
    } catch (error) {
        console.error('Chart rendering failed:', error);
        showFallback('waitTimeChart');
    }
};

window.renderTreatmentTimeChart = function(data) {
    const canvas = getValidCanvas('treatmentTimeChart');
    if (!canvas) return;
    
    destroyChart('treatmentTimeChart');
    
    try {
        const safeData = safeArray(data);
        const ctx = canvas.getContext('2d');
        chartInstances['treatmentTimeChart'] = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: safeData.map(d => safeString(d.range, 'Unknown')),
                datasets: [{
                    label: 'Number of Patients',
                    data: safeData.map(d => safeValue(d.count, 0)),
                    backgroundColor: 'rgba(75, 192, 192, 0.6)',
                    borderColor: 'rgba(75, 192, 192, 1)',
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            stepSize: 1
                        }
                    }
                },
                plugins: {
                    legend: {
                        display: false
                    }
                }
            }
        });
    } catch (error) {
        console.error('Chart rendering failed:', error);
        showFallback('treatmentTimeChart');
    }
};

window.renderTreatmentTypesChart = function(data) {
    const canvas = getValidCanvas('treatmentTypesChart');
    if (!canvas) return;
    
    destroyChart('treatmentTypesChart');
    
    try {
        const safeData = safeArray(data);
        const ctx = canvas.getContext('2d');
        chartInstances['treatmentTypesChart'] = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: safeData.map(d => safeString(d.treatmentName, 'Unspecified')),
                datasets: [{
                    label: 'Number of Patients',
                    data: safeData.map(d => safeValue(d.patientCount, 0)),
                    backgroundColor: 'rgba(255, 99, 132, 0.6)',
                    borderColor: 'rgba(255, 99, 132, 1)',
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            stepSize: 1
                        }
                    }
                },
                plugins: {
                    legend: {
                        display: false
                    }
                }
            }
        });
    } catch (error) {
        console.error('Chart rendering failed:', error);
        showFallback('treatmentTypesChart');
    }
};

window.renderCounsellorChart = function(data) {
    const canvas = getValidCanvas('counsellorChart');
    if (!canvas) return;
    
    destroyChart('counsellorChart');
    
    try {
        const safeData = safeArray(data);
        const ctx = canvas.getContext('2d');
        chartInstances['counsellorChart'] = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: safeData.map(d => safeString(d.counsellorName, 'Unspecified')),
                datasets: [{
                    label: 'Number of Patients',
                    data: safeData.map(d => safeValue(d.patientCount, 0)),
                    backgroundColor: 'rgba(153, 102, 255, 0.6)',
                    borderColor: 'rgba(153, 102, 255, 1)',
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            stepSize: 1
                        }
                    }
                },
                plugins: {
                    legend: {
                        display: false
                    }
                }
            }
        });
    } catch (error) {
        console.error('Chart rendering failed:', error);
        showFallback('counsellorChart');
    }
};

window.renderLanguageChart = function(demographics) {
    const canvas = getValidCanvas('languageChart');
    if (!canvas) return;
    
    destroyChart('languageChart');
    
    try {
        // Handle null demographics object
        const safeDemographics = demographics || {};
        const ctx = canvas.getContext('2d');
        chartInstances['languageChart'] = new Chart(ctx, {
            type: 'pie',
            data: {
                labels: ['English First Language', 'Other Language'],
                datasets: [{
                    data: [
                        safeValue(safeDemographics.englishFirstLanguageCount, 0), 
                        safeValue(safeDemographics.otherLanguageCount, 0)
                    ],
                    backgroundColor: [
                        'rgba(54, 162, 235, 0.8)',
                        'rgba(255, 206, 86, 0.8)'
                    ],
                    borderColor: [
                        'rgba(54, 162, 235, 1)',
                        'rgba(255, 206, 86, 1)'
                    ],
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'bottom'
                    }
                }
            }
        });
    } catch (error) {
        console.error('Chart rendering failed:', error);
        showFallback('languageChart');
    }
};

window.renderSurveyChart = function(demographics) {
    const canvas = getValidCanvas('surveyChart');
    if (!canvas) return;
    
    destroyChart('surveyChart');
    
    try {
        // Handle null demographics object
        const safeDemographics = demographics || {};
        const ctx = canvas.getContext('2d');
        chartInstances['surveyChart'] = new Chart(ctx, {
            type: 'pie',
            data: {
                labels: ['Survey Returned', 'Survey Not Returned'],
                datasets: [{
                    data: [
                        safeValue(safeDemographics.surveyReturnedCount, 0), 
                        safeValue(safeDemographics.surveyNotReturnedCount, 0)
                    ],
                    backgroundColor: [
                        'rgba(75, 192, 192, 0.8)',
                        'rgba(255, 99, 132, 0.8)'
                    ],
                    borderColor: [
                        'rgba(75, 192, 192, 1)',
                        'rgba(255, 99, 132, 1)'
                    ],
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'bottom'
                    }
                }
            }
        });
    } catch (error) {
        console.error('Chart rendering failed:', error);
        showFallback('surveyChart');
    }
};

window.renderTrendsChart = function(data) {
    const canvas = getValidCanvas('trendsChart');
    if (!canvas) return;
    
    destroyChart('trendsChart');
    
    try {
        const safeData = safeArray(data);
        const ctx = canvas.getContext('2d');
        chartInstances['trendsChart'] = new Chart(ctx, {
            type: 'line',
            data: {
                labels: safeData.map(d => safeString(d.period, 'Unknown')),
                datasets: [{
                    label: 'Referrals',
                    data: safeData.map(d => safeValue(d.count, 0)),
                    backgroundColor: 'rgba(255, 159, 64, 0.2)',
                    borderColor: 'rgba(255, 159, 64, 1)',
                    borderWidth: 2,
                    fill: true,
                    tension: 0.1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            stepSize: 1
                        }
                    }
                },
                plugins: {
                    legend: {
                        display: false
                    }
                }
            }
        });
    } catch (error) {
        console.error('Chart rendering failed:', error);
        showFallback('trendsChart');
    }
};

