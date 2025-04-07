const cardModal = document.getElementById('cardModal');
cardModal.addEventListener('show.bs.modal', function (event) {
    const button = event.relatedTarget;

    const title = button.getAttribute('data-bs-title');
    const img = button.getAttribute('data-bs-img');
    const text = button.getAttribute('data-bs-text');

    const modalTitle = cardModal.querySelector('.modal-title');
    const modalImg = cardModal.querySelector('#cardModalImg');
    const modalText = cardModal.querySelector('#cardModalText');

    modalTitle.textContent = title;
    modalImg.src = img;
    modalText.textContent = text;
});
