# SYNOP Datenparser
# Parst synoptische Meldungen von Wetterstationen und liefert die gemessenen Werte als
# Dataframe zurück.
# (c) Michael Schletz, 2022
import re, bz2
import pandas as pd
import datetime as dt

lineExp = re.compile(
    "(?P<station>\d+),(?P<year>\d{4}),(?P<month>\d{2}),(?P<day>\d{2}),(?P<hour>\d{2}),(?P<minute>\d{2}),(?P<data>.+)"
)
dataExp = re.compile(
    "(?P<id_string>\w+) (?P<day>\d{2})(?P<hour>\d{2})(?P<wind_unit>\d) (?P<id>\d+) .{5} ((?P<cloud_octas>\d)|/)((?P<wind_dir>\d{2})|//)((?P<wind_speed>\d{2})|//) 1(?P<temp>\d{4}) 2(?P<dewp>\d{4}) 3(?P<pressure_station>\d{4})(.* 6(?P<precip>\d{4}))?(.* 333 (?P<ext_data>.+)=)?"
)
extDataExp = re.compile("(.*1(?P<max_temp>\d{4}))?(.*2(?P<min_temp>\d{4}))?(.*553(?P<sunshine>\d{2}))?")


def parse_data(hour, data):
    """
    Parst den Datenteil einer Zeile, z. B. AAXX 01034 11180 46/// /3302 11084 21087 38431 48490=
    :return: Tupel mit Tepmeratur, Taupunkt, Druck, Bewölkung, Wind, Max. Temp, Min. Temp, Sonne
    """
    match = dataExp.match(data)
    if match is None:
        return None
    temp = int(match.group("temp"))
    temp = (-(temp - 1000) if temp >= 1000 else temp) / 10

    dewp = int(match.group("dewp"))
    dewp = (-(dewp - 1000) if dewp >= 1000 else dewp) / 10

    pressure_station = int(match.group("pressure_station"))
    pressure_station = (pressure_station if pressure_station >= 5000 else 10000 + pressure_station) / 10

    if match.group("pressure_station") is not None:
        pressure_sea = int(match.group("pressure_station"))
        pressure_sea = (pressure_sea if pressure_sea >= 5000 else 10000 + pressure_sea) / 10
    else:
        pressure_sea = None
    # Niederschlag (precipiation)
    if match.group("precip") is not None:
        precip = int(match.group("precip"))
        prec_amount = int(precip / 10)
        prec_amount = (prec_amount-990)/10 if prec_amount >= 990 else prec_amount
        prec_duration = precip % 10
        prec_duration = 12 if prec_duration == 2 else 6 if prec_duration == 1 else 3 if prec_duration == 7 else 1 if prec_duration == 5 else None
    else:
        prec_amount = prec_duration = None

    ext_data = parse_ext_data(hour, match.group("ext_data"))
    return (
        temp,
        dewp,
        pressure_station,
        prec_amount,
        prec_duration,
        int(match.group("cloud_octas")) if match.group("cloud_octas") is not None else None,
        int(match.group("wind_dir")) if match.group("wind_dir") is not None else None,
        int(match.group("wind_speed")) if match.group("wind_speed") is not None else None,
        ext_data[0],
        ext_data[1],
        ext_data[2],
    )


def parse_ext_data(hour, data):
    """
    Parst den erweiterten Block (nach 333) pro Zeile
    """
    if data is None:
        return (None, None, None)
    match = extDataExp.match(data)
    if match is None:
        return (None, None, None)
    if match.group("max_temp") is not None and hour == 18:
        max_temp = int(match.group("max_temp"))
        max_temp = (-(max_temp - 1000) if max_temp >= 1000 else max_temp) / 10
    else:
        max_temp = None
    if match.group("min_temp") is not None and hour == 6:
        min_temp = int(match.group("min_temp"))
        min_temp = (-(min_temp - 1000) if min_temp >= 1000 else min_temp) / 10
    else:
        min_temp = None
    return (max_temp, min_temp, int(match.group("sunshine")) * 6 if match.group("sunshine") is not None else None)


def readFile(filename, maxLines=1000000):
    result = []
    with bz2.open(filename, mode="rt") as file:
        while True:
            line = file.readline()
            if not line:
                break
            maxLines = maxLines - 1
            match = lineExp.match(line)
            if match is None:
                continue

            station = int(match.group("station"))
            year = int(match.group("year"))
            month = int(match.group("month"))
            day = int(match.group("day"))
            hour = int(match.group("hour"))
            minute = int(match.group("minute"))
            data = parse_data(hour, match.group("data"))
            if data is None:
                continue

            date = dt.datetime(year, month, day)
            datetime = dt.datetime(year, month, day, hour, minute, 0, 0)
            result.append(
                (
                    station,
                    date,
                    datetime,
                    year,
                    month,
                    day,
                    hour,
                    minute,
                    data[0],
                    data[1],
                    data[2],
                    data[3],
                    data[4],
                    data[5],
                    data[6],
                    data[7],
                    data[8],
                    data[9],
                    data[10]
                )
            )
            if maxLines == 0:
                break
    df = pd.DataFrame(
        data=result,
        columns=[
            "station",
            "date",
            "datetime",
            "year",
            "month",
            "day",
            "hour",
            "minute",
            "temp",
            "dewp",
            "pressure",
            "prec_amount",
            "prec_duration",
            "cloud_octas",
            "wind_dir",
            "wind_speed",
            "max_temp",
            "min_temp",
            "sunshine",
        ],
    )
    return df
