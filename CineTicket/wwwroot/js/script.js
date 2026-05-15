const container = document.querySelector('.seat-container');
if (container) {
    const rowCount = 15;
    const seatCount = 12;
    const alphabet = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O'];

    const emptyCorner = document.createElement('div');
    container.appendChild(emptyCorner);

    for (let i = 1; i <= seatCount; i++) {
        const numberDiv = document.createElement('div');
        numberDiv.classList.add('col-number');
        numberDiv.innerText = i;
        container.appendChild(numberDiv);
    }

    for (let r = 0; r < rowCount; r++) {
        const letterDiv = document.createElement('div');
        letterDiv.classList.add('row-label');
        letterDiv.innerText = alphabet[r];
        container.appendChild(letterDiv);

        for (let s = 1; s <= seatCount; s++) {
            const seat = document.createElement('div');
            seat.classList.add('seat');
            seat.setAttribute('data-seat', `${alphabet[r]}-${s}`);

            // if (Math.random() < 0.1) {
            //     seat.classList.add('occupied');
            // }

            container.appendChild(seat);
        }
    }

    container.addEventListener('click', (e) => {
        if (e.target.classList.contains('seat') && !e.target.classList.contains('occupied')) {
            e.target.classList.toggle('selected');
            updateSummary();
        }
    });

    const selectCity = document.getElementById('select-city');
    const selectCounty = document.getElementById('select-county');
    const selectTime = document.getElementById('select-time');
    const selectHall = document.getElementById('select-hall');

    const infoText = document.getElementById('selection-info');
    const selectedSeatsSpan = document.getElementById('selected-seats');
    const totalPriceSpan = document.getElementById('total-price');
    const ticketPrice = 4.99;

    function updateSummary() {
        const selectedSeats = document.querySelectorAll('.seat.selected');
        const seatIndex = [...selectedSeats].map(seat => seat.getAttribute('data-seat'));

        if (selectedSeats.length > 0) {
            selectedSeatsSpan.innerText = seatIndex.join(', ');
            totalPriceSpan.innerText = (selectedSeats.length * ticketPrice).toFixed(2) + '$';
        } else {
            selectedSeatsSpan.innerText = '-';
            totalPriceSpan.innerText = '0$';
        }

        const city = selectCity.value !== '' ? selectCity.value : '...';
        const county = selectCounty.value !== '' ? selectCounty.value : '...';
        const time = selectTime.value !== '' ? selectTime.value : '...';
        const hall = selectHall.value !== '' ? selectHall.value : '...';

        infoText.innerText = `${city} / ${county} / ${time} / ${hall}`;
    }

    selectCounty.addEventListener('change', updateSummary);
    selectTime.addEventListener('change', updateSummary);
    selectHall.addEventListener('change', updateSummary);

    const cityData = {
        'Arizona': ['Maricopa', 'Pima', 'Yavapai'],
        'California': ['Los Angeles', 'San Diego', 'Orange', 'San Francisco'],
        'Florida': ['Miami-Dade', 'Broward', 'Orange', 'Hillsborough'],
        'Georgia': ['Fulton', 'DeKalb', 'Cobb'],
        'Illinois': ['Cook', 'DuPage', 'Lake'],
        'Michigan': ['Wayne', 'Oakland', 'Macomb'],
        'New Jersey': ['Bergen', 'Essex', 'Hudson'],
        'New York': ['Manhattan', 'Brooklyn', 'Queens', 'Bronx'],
        'North Carolina': ['Mecklenburg', 'Wake', 'Durham'],
        'Ohio': ['Franklin', 'Cuyahoga', 'Hamilton'],
        'Pennsylvania': ['Philadelphia', 'Allegheny', 'Montgomery'],
        'Texas': ['Harris', 'Dallas', 'Travis', 'Bexar'],
        'Virginia': ['Fairfax', 'Arlington', 'Loudoun'],
        'Washington': ['King', 'Pierce', 'Snohomish']
    };

    selectCity.addEventListener('change', (e) => {
        const selectedCity = e.target.value;

        selectCounty.innerHTML = '<option value="" selected>Select County</option>';

        if (selectedCity !== '' && cityData[selectedCity]) {
            selectCounty.disabled = false;
            const counties = cityData[selectedCity];

            counties.forEach(county => {
                const option = document.createElement('option');
                option.value = county;
                option.innerText = county;
                selectCounty.appendChild(option);
            });
        } else {
            selectCounty.disabled = true;
        }
        updateSummary();
    });

    //const buyButton = document.getElementById('buy-button');
    //buyButton.addEventListener('click', () => {
    //    const selectedSeatsCount = document.querySelectorAll('.seat.selected').length;
    //    if (selectCity.value === '' || selectCounty.value === '' ||
    //        selectTime.value === '' || selectHall.value === '') {
    //        alert('Please fill in all the options (State, County, Time, Hall)!');
    //        return;
    //    }
    //    if (selectedSeatsCount === 0) {
    //        alert('Please choose at least one seat!');
    //        return;
    //    }
    //    window.location.href = window.location.pathname;
    //});
    const buyButton = document.getElementById('buy-button');
    if (buyButton) {
        buyButton.addEventListener('click', (e) => {
            e.preventDefault();

            const selectedSeatsCount = document.querySelectorAll('.seat.selected').length;
            if (selectCity.value === '' || selectCounty.value === '' || selectTime.value === '' || selectHall.value === '') {
                alert('Please fill in all the options (State, County, Time, Hall)!');
                return;
            }
            if (selectedSeatsCount === 0) {
                alert('Please choose at least one seat!');
                return;
            }

            const movieIdEl = document.getElementById('movie-id');
            if (!movieIdEl) {
                alert("Paga is not found!");
                return;
            }
            const movieId = movieIdEl.value;
            const city = selectCity.value + ", " + selectCounty.value;
            const hall = selectHall.value;
            const time = selectTime.value;

            const selectedSeatsNode = document.querySelectorAll('.seat.selected');
            const seatArray = [...selectedSeatsNode].map(seat => seat.getAttribute('data-seat'));
            const seatString = seatArray.join(', ');

            const price = parseFloat((selectedSeatsCount * ticketPrice).toFixed(2));


            const ticketData = {
                MovieId: parseInt(movieId),
                City: city,
                Hall: hall,
                ShowTime: time,
                SeatNumbers: seatString,
                Price: price
            };

            fetch('/Home/BuyTicket', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(ticketData)
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        alert("The ticket was successfully purchased! You are being redirected to your profile...");
                        window.location.href = "/Account/Profile?tab=tickets";
                    } else {
                        alert("Error: " + data.message);
                    }
                })
                .catch(error => {
                    console.error('Error:', error);
                    alert("A systemic error has occurred.");
                });
        });
    }



    const messageInput = document.getElementById('message');
    const sendBtn = document.getElementById('sendBtn');
    const messageArea = document.getElementById('messageArea');

    if (sendBtn) {
        sendBtn.addEventListener('click', () => {

            const message = messageInput.value;

            if (message.trim() !== '') {

                const newDiv = document.createElement('div');

                newDiv.className = "border mt-3 py-3 border-danger container rounded-2";

                newDiv.innerHTML = `
            <h5 class="text-warning">Guest User</h5>
            <p class="text-white mt-3 mb-0">${message}</p>
        `;
                messageArea.appendChild(newDiv);
                messageInput.value = '';
            }
        });
    }
}

const menuItems = document.querySelectorAll('.list-group-item');

if (menuItems.length > 0) {

    const sections = document.querySelectorAll(
        '#personal-section, #tickets-section, #comments-section, #password-section'
    );

    menuItems.forEach(item => {
        item.addEventListener('click', function (e) {
            e.preventDefault();

            sections.forEach(section => {
                section.classList.add('d-none');
            });

            menuItems.forEach(menu => {
                menu.classList.remove('active');
            });

            this.classList.add('active');

            const targetId = this.id + '-section';
            const targetSection = document.getElementById(targetId);

            if (targetSection) {
                targetSection.classList.remove('d-none');
            }
        });
    });
}

const time = document.getElementById('time');
const timeCalculator = document.getElementById('time-calculator');

if (time && timeCalculator) {
    time.addEventListener('input', () => {

        const timeValue = time.value;
        const hour = Math.floor(timeValue / 60);
        const minutes = timeValue % 60;

        timeCalculator.textContent = `${hour}h ${minutes}m`;
    });
}

const adminMenuItems = document.querySelectorAll('.list-group-item');

if (adminMenuItems.length > 0) {
    const adminSections = document.querySelectorAll('#film-section, #comments-section, #users-section');
    adminMenuItems.forEach(item => {
        item.addEventListener('click', function (e) {
            e.preventDefault();

            adminSections.forEach(adminsection => {
                adminsection.classList.add('d-none');
            });

            adminMenuItems.forEach(adminmenu => {
                adminmenu.classList.remove('active');
            });

            this.classList.add('active');

            const targetId = this.id + '-section';
            const targetSection = document.getElementById(targetId);

            if (targetSection) {
                targetSection.classList.remove('d-none');
            }
        });
    });
}
function filterComments() {

    var selectedMovieId = document.getElementById("movieFilter").value;

    var comments = document.getElementsByClassName("comment-card");

    for (var i = 0; i < comments.length; i++) {
        var card = comments[i];
        var cardMovieId = card.getAttribute("data-movie-id");

        if (selectedMovieId === "all") {

            card.style.display = "block";

        } else {

            if (cardMovieId === selectedMovieId) {
                card.style.display = "block";
            } else {
                card.style.display = "none";
            }
        }
    }
}

document.addEventListener("DOMContentLoaded", function () {

    const messageInput = document.getElementById('has-password-message');
    if (messageInput && messageInput.value === 'true') {
        const passBtn = document.getElementById('password');
        if (passBtn) {
            passBtn.click();
        }
    }

    const urlParams = new URLSearchParams(window.location.search);
    const targetTab = urlParams.get('tab');

    if (targetTab === 'tickets') {
        const ticketBtn = document.getElementById('tickets');
        if (ticketBtn) {
            ticketBtn.click();
        }
    }
    else if (targetTab === 'comments') {
        const commentBtn = document.getElementById('comments');
        if (commentBtn) commentBtn.click();
    }
});